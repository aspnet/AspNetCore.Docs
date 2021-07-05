---
title: gRPC client-side load balancing
author: jamesnk
description: Learn how to make scalable, high-performance gRPC apps with client-side load balancing in .NET.
monikerRange: '>= aspnetcore-3.0'
ms.author: jamesnk
ms.date: 05/18/2021
no-loc: [Home, Privacy, Kestrel, appsettings.json, "ASP.NET Core Identity", cookie, Cookie, Blazor, "Blazor Server", "Blazor WebAssembly", "Identity", "Let's Encrypt", Razor, SignalR, "service config"]
uid: grpc/loadbalancing
---
# gRPC client-side load balancing

By [James Newton-King](https://twitter.com/jamesnk)

Client-side load balancing is a feature that allows gRPC clients to distribute load optimally across available servers. This article discusses how to configure client-side load balancing to create scalable, high-performance gRPC apps in .NET.

Client-side load balancing requires:

* .NET 5 or later.
* [Grpc.Net.Client](https://www.nuget.org/packages/Grpc.Net.Client) version 2.39.0-pre1 or later.

> [!IMPORTANT]
> This feature is in preview. Integration with other gRPC features is not complete and testing is required.
> 
> Client-side load balancing is currently:
> 
> * Only available in pre-release versions of `Grpc.Net.Client` on NuGet.org.
> * Not supported when used with [gRPC client factory](xref:grpc/clientfactory).

## Configure gRPC client-side load balancing

Client-side load balancing is configured when a channel is created. The two components to consider when using load balancing:

* The resolver, which resolves the addresses for the channel. Resolvers support getting addresses from an external source. This is also known as service discovery.
* The load balancer, which creates connections and picks the address that a gRPC call will use.

Built-in implementations of resolvers and load balancers are included in [Grpc.Net.Client](https://www.nuget.org/packages/Grpc.Net.Client). Load balancing can also be extended by [writing custom resolvers and load balancers](#write-custom-resolvers-and-load-balancers).

Addresses, connections and other load balancing state is stored in a `GrpcChannel` instance. A channel must be reused when making gRPC calls for load balancing to work correctly.

## Configure resolver

The resolver is configured using the address a channel is created with. The [URI scheme](https://wikipedia.org/wiki/Uniform_Resource_Identifier#Syntax) of the address specifies the resolver.

| Scheme   | Type                    | Description |
| -------- | ----------------------- | ----------- |
| `dns`    | `DnsResolverFactory`    | Resolves addresses by querying the hostname for [DNS service records](https://en.wikipedia.org/wiki/SRV_record). |
| `static` | `StaticResolverFactory` | Resolves addresses that the app has specified. Recommended if an app already knows the addresses it calls. |

A channel doesn't directly call a URI that matches a resolver. Instead, a matching resolver is created and used to resolve the addresses.

For example, using `GrpcChannel.ForAddress("dns:///my-example-host", new GrpcChannelOptions { ChannelCredentials = ChannelCredentials.Insecure })`:

* The `dns` scheme maps to `DnsResolverFactory`. A new instance of a DNS resolver is created for the channel.
* The resolver makes a DNS query for `my-example-host` and gets two results: `localhost:80` and `localhost:81`.
* The load balancer uses `localhost:80` and `localhost:81` to create connections and make gRPC calls.

#### DnsResolverFactory

The `DnsResolverFactory` creates a resolver designed to get addresses from an external source. DNS resolution is commonly used to load balance over pod instances that have a [Kubernetes headless services](https://kubernetes.io/docs/concepts/services-networking/service/#headless-services).

```csharp
var channel = GrpcChannel.ForAddress(
    "dns:///my-example-host",
    new GrpcChannelOptions { ChannelCredentials = ChannelCredentials.Insecure });
var client = new Greet.GreeterClient(channel);

var response = await client.SayHelloAsync(new HelloRequest { Name = "world" });
```

The preceding code:

* Configures the created channel with the address `dns:///my-example-host`. The `dns` scheme maps to `DnsResolverFactory`.
* Doesn't specify a load balancer. The channel defaults to a pick first load balancer.
* Starts the gRPC call `SayHello`:
  * DNS resolver gets addresses for the hostname `my-example-host`.
  * Pick first load balancer attempts to connect to one of the resolved addresses.
  * The call is sent to the first address the channel successfully connects to.

Performance is important when load balancing. The latency of resolving addresses is eliminated from gRPC calls by caching the addresses. A resolver will be invoked when making the first gRPC call, and subsequent calls use the cache. Addresses are automatically refreshed if a connection is interrupted. Refreshing is important in scenarios where addresses change at runtime. For example, in Kubernetes a [restarted pod](https://kubernetes.io/docs/concepts/workloads/pods/pod-lifecycle/) triggers the DNS resolver to refresh and get the pod's new address.

#### StaticResolverFactory

A static resolver is provided by `StaticResolverFactory`. This resolver:

* Doesn't call an external source. Instead, the client app configures the addresses.
* Is designed for situations where an app already knows the addresses it calls.

```csharp
var factory = new StaticResolverFactory(addr => new[]
{
    new DnsEndPoint("localhost", 80),
    new DnsEndPoint("localhost", 81)
});

var services = new ServiceCollection();
services.AddSingleton<ResolverFactory>(factory);

var channel = GrpcChannel.ForAddress(
    "static:///my-example-host",
    new GrpcChannelOptions
    {
        ChannelCredentials = ChannelCredentials.Insecure,
        ServiceProvider = services.BuildServiceProvider()
    });
var client = new Greet.GreeterClient(channel);
```

The preceding code:

* Creates a `StaticResolverFactory`. This factory knows about two addresses: `localhost:80` and `localhost:81`.
* Registers the factory with dependency injection (DI).
* Configures the created channel with:
  * The address `static:///my-example-host`. The `static` scheme maps to a static resolver.
  * Sets `GrpcChannelOptions.ServiceProvider` with the DI service provider.

This example creates a new <xref:Microsoft.Extensions.DependencyInjection.ServiceCollection> for DI. Suppose an app already has DI setup, like an ASP.NET Core website. In that case, types should be registered with the existing DI instance. `GrpcChannelOptions.ServiceProvider` is configured by getting an <xref:System.IServiceProvider> from DI.

## Configure load balancer

A load balancer is specified in a [service config](https://github.com/grpc/grpc/blob/master/doc/service_config.md) using the `ServiceConfig.LoadBalancingConfigs` collection. Two load balancers are built-in and map to load balancer config names:

| Name          | Type                            | Description |
| ------------- | ------------------------------- | ----------- |
| `pick_first`  | `PickFirstLoadBalancerFactory`  | Attempts to connect to addresses until a connection is successfully made. gRPC calls are all made to the first successful connection. |
| `round_robin` | `RoundRobinLoadBalancerFactory` | Attempts to connect to all addresses. gRPC calls are distributed across all successful connections using [round-robin](https://www.nginx.com/resources/glossary/round-robin-load-balancing/) logic. |

Service config is an abbreviation of service configuration and is represented by the `ServiceConfig` type. There are a couple of ways a channel can get a service config with a load balancer configured:

* An app can specify a service config when a channel is created using `GrpcChannelOptions.ServiceConfig`.
* Alternatively, a resolver can resolve a service config for a channel. This feature allows an external source to specify how its callers should perform load balancing. Whether a resolver supports resolving a service config is dependent on the resolver implementation. Disable this feature with `GrpcChannelOptions.DisableResolverServiceConfig`.
* If no service config is provided, or the service config doesn't have a load balancer configured, the channel defaults to `PickFirstLoadBalancerFactory`.

```csharp
var channel = GrpcChannel.ForAddress(
    "dns:///my-example-host",
    new GrpcChannelOptions
    {
        ChannelCredentials = ChannelCredentials.Insecure,
        ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new RoundRobinConfig() } }
    });
var client = new Greet.GreeterClient(channel);

var response = await client.SayHelloAsync(new HelloRequest { Name = "world" });
```

The preceding code:

* Specifies a `RoundRobinLoadBalancerFactory` in the service config.
* Starts the gRPC call `SayHello`:
  * `DnsResolverFactory` creates a resolver that gets addresses for the hostname `my-example-host`.
  * Round-robin load balancer attempts to connect to all resolved addresses.
  * gRPC calls are distributed evenly using round-robin logic.

## Configure channel credentials

A channel must know whether gRPC calls are sent using [transport security](xref:grpc/security#transport-security). `http` and `https` are no longer part of the address, the scheme now specifies a resolver, so `ChannelCredentials` must be configured on channel options when using load balancing.

* `ChannelCredentials.SecureSsl` - gRPC calls are secured with [Transport Layer Security (TLS)](https://tools.ietf.org/html/rfc5246). Equivalent to an `https` address.
* `ChannelCredentials.Insecure` - gRPC calls don't use transport security. Equivalent to an `http` address.

```csharp
var channel = GrpcChannel.ForAddress(
    "dns:///my-example-host",
    new GrpcChannelOptions { ChannelCredentials = ChannelCredentials.Insecure });
var client = new Greet.GreeterClient(channel);

var response = await client.SayHelloAsync(new HelloRequest { Name = "world" });
```

## Write custom resolvers and load balancers

Client-side load balancing is extensible:

* Implement `Resolver` to create a custom resolver and resolve addresses from a new data source.
* Implement `LoadBalancer` to create a custom load balancer with new load balancing behavior.

> [!IMPORTANT]
> The APIs used to extend client-side load balancing are experimental. They can change without notice.

### Create a custom resolver

A resolver:

* Implements `Resolver` and is created by a `ResolverFactory`. Create a custom resolver by implementing these types.
* Is responsible for resolving the addresses a load balancer uses.
* Can optionally provide a service configuration.

```csharp
public class FileResolver : Resolver
{
    private readonly Uri _address;
    private Action<ResolverResult> _listener;

    public ExampleResolver(Uri address)
    {
        _address = address;
    }

    public override async Task RefreshAsync(CancellationToken cancellationToken)
    {
        // Load JSON from a file on disk and deserialize into endpoints.
        var jsonString = await File.ReadAllTextAsync(_address.LocalPath);
        var results = JsonSerializer.Deserialize<string[]>(jsonString);
        var addresses = results.Select(r => new DnsEndPoint(r, 80));

        // Pass the results back to the channel.
        _listener(ResolverResult.ForResult(addresses, serviceConfig: null));
    }

    public override void Start(Action<ResolverResult> listener)
    {
        _listener = listener;
    }
}

public class FileResolverFactory : ResolverFactory
{
    // Create a FileResolver when the URI has a 'file' scheme.
    public override string Name => "file";

    public override Resolver Create(ResolverOptions options)
    {
        return new FileResolver(options.Address);
    }
}
```

In the preceding code:

* `FileResolverFactory` implements `ResolverFactory`. It maps to the `file` scheme and creates `FileResolver` instances.
* `FileResolver` implements `Resolver`. In `RefreshAsync`:
  * The file URI is converted to a local path. For example, `file:///c:/addresses.json` becomes `c:\addresses.json`.
  * JSON is loaded from disk and converted into a collection of addresses.
  * Listener is called with results to let the channel know that addresses are available.

### Create a custom load balancer

A load balancer:

* Implements `LoadBalancer` and is created by a `LoadBalancerFactory`. Create a custom load balancer and factory by implementing these types.
* Is given addresses from a resolver and creates `Subchannel` instances.
* Tracks state about the connection and creates a `SubchannelPicker`. The channel internally uses the picker to pick addresses when making gRPC calls.

The `SubchannelsLoadBalancer` is:

* An abstract base class that implements `LoadBalancer`.
* Manages creating `Subchannel` instances from addresses.
* Makes it easy to implement a custom picking policy over a collection of subchannels.

```csharp
public class RandomBalancer : SubchannelsLoadBalancer
{
    public RandomBalancer(IChannelControlHelper controller, ILoggerFactory loggerFactory)
        : base(controller, loggerFactory)
    {
    }

    protected override SubchannelPicker CreatePicker(List<Subchannel> readySubchannels)
    {
        return new RandomPicker(readySubchannels);
    }

    private class RandomPicker : SubchannelPicker
    {
        private readonly List<Subchannel> _subchannels;

        public RandomPicker(List<Subchannel> subchannels)
        {
            _subchannels = readySubchannels;
        }

        public override PickResult Pick(PickContext context)
        {
            // Pick a random subchannel.
            return PickResult.ForSubchannel(_subchannels[Random.Shared.Next(0, _subchannels.Count)]);
        }
    }
}

public class RandomBalancerFactory : LoadBalancerFactory
{
    // Create a RandomBalancer when the name is 'random'.
    public override string Name => "random";

    public override LoadBalancer Create(LoadBalancerOptions option)
    {
        return new RandomBalancer(options.Controller, options.LoggerFactory);
    }
}
```

In the preceding code:

* `RandomBalancerFactory` implements `LoadBalancerFactory`. It maps to the `random` policy name and creates `RandomBalancer` instances.
* `RandomBalancer` implements `SubchannelsLoadBalancer`. It creates a `RandomPicker` that randomly picks a subchannel.

## Configure custom resolvers and load balancers

Custom resolvers and load balancers need to be registered with dependency injection (DI) when they are used. There are a couple of options:

* If an app is already using DI, such as an ASP.NET Core web app, they can be registered with the existing DI configuration. An <xref:System.IServiceProvider> can be resolved from DI and passed to the channel using `GrpcChannelOptions.ServiceProvider`.
* If an app isn't using DI then create:
  * A <xref:Microsoft.Extensions.DependencyInjection.ServiceCollection> with types registered with it.
  * A service provider using <xref:Microsoft.Extensions.DependencyInjection.ServiceCollectionContainerBuilderExtensions.BuildServiceProvider*>.

```csharp
var services = new ServiceCollection();
services.AddSingleton<ResolverFactory, FileResolverFactory>();
services.AddSingleton<LoadBalancerFactory, RandomLoadBalancerFactory>();

var channel = GrpcChannel.ForAddress(
    "file:///c:/data/addresses.json",
    new GrpcChannelOptions
    {
        ChannelCredentials = ChannelCredentials.Insecure,
        ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new LoadBalancingConfig("random") } },
        ServiceProvider = services.BuildServiceProvider()
    });
var client = new Greet.GreeterClient(channel);
```

The preceding code:

* Creates a `ServiceCollection` and registers new resolver and load balancer implementations.
* Creates a channel configured to use the new implementations:
  * `ServiceCollection` is built into an `IServiceProvider` and set to `GrpcChannelOptions.ServiceProvider`.
  * Channel address is `file:///c:/data/addresses.json`. The `file` scheme maps to `FileResolverFactory`.
  * Service config load balancer name is `random`. Maps to `RandomLoadBalancerFactory`.

## Why load balancing is important

HTTP/2 multiplexes multiple calls on a single TCP connection. If gRPC and HTTP/2 are used with a network load balancer (NLB), the connection is forwarded to a server, and all gRPC calls are sent to that one server. The other server instances on the NLB are idle.

Network load balancers are a common solution for load balancing because they are fast and lightweight. For example, Kubernetes by default uses a network load balancer to balance connections between pod instances. However, network load balancers are not effective at distributing load when used with gRPC and HTTP/2.

### Proxy or client-side load balancing?

gRPC and HTTP/2 can be effectively load balanced using either an application load balancer proxy or client-side load balancing. Both of these options allow individual gRPC calls to be distributed across available servers. Deciding between proxy and client-side load balancing is an architectural choice. There are pros and cons for each.

* **Proxy**: gRPC calls are sent to the proxy, the proxy makes a load balancing decision, and the gRPC call is sent on to the final endpoint. The proxy is responsible for knowing about endpoints. Using a proxy adds:

  * An additional network hop to gRPC calls.
  * Latency and consumes additional resources.
  * Proxy server must be setup and configured correctly.

* **Client-side load balancing**: The gRPC client makes a load balancing decision when a gRPC call is started. The gRPC call is sent directly to the final endpoint. When using client-side load balancing:

  * The client is responsible for knowing about available endpoints and making load balancing decisions.
  * Additional client configuration is required.
  * High-performance, load balanced gRPC calls eliminate the need for a proxy.

## Additional resources

* <xref:grpc/client>
