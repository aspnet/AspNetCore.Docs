---
title: ASP.NET Core SignalR JavaScript client
author: bradygaster
description: Overview of ASP.NET Core SignalR JavaScript client.
monikerRange: '>= aspnetcore-2.1'
ms.author: bradyg
ms.custom: mvc, devx-track-js
ms.date: 04/08/2020
no-loc: [Home, Privacy, Kestrel, appsettings.json, "ASP.NET Core Identity", cookie, Cookie, Blazor, "Blazor Server", "Blazor WebAssembly", "Identity", "Let's Encrypt", Razor, SignalR]
uid: signalr/javascript-client
---
# ASP.NET Core SignalR JavaScript client

::: moniker range=">= aspnetcore-3.0"

By [Rachel Appel](https://twitter.com/rachelappel)

The ASP.NET Core SignalR JavaScript client library enables developers to call server-side hub code.

[View or download sample code](https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/signalr/javascript-client/samples) ([how to download](xref:index#how-to-download-a-sample))

## Install the SignalR client package

The SignalR JavaScript client library is delivered as an [npm](https://www.npmjs.com/) package. The following sections outline different ways to install the client library.

### Install with npm

For Visual Studio, run the following commands from **Package Manager Console** while in the root folder. For Visual Studio Code, run the following commands from the **Integrated Terminal**.

```bash
npm init -y
npm install @microsoft/signalr
```

npm installs the package contents in the *node_modules\\@microsoft\signalr\dist\browser* folder. Create a new folder named *signalr* under the *wwwroot\\lib* folder. Copy the *signalr.js* file to the *wwwroot\lib\signalr* folder.

Reference the SignalR JavaScript client in the `<script>` element. For example:

```html
<script src="~/lib/signalr/signalr.js"></script>
```

### Use a Content Delivery Network (CDN)

To use the client library without the npm prerequisite, reference a CDN-hosted copy of the client library. For example:

[!code-html[](javascript-client/samples/3.x/SignalRChat/Pages/Index.cshtml?name=snippet_CDN)]

The client library is available on the following CDNs:

* [cdnjs](https://cdnjs.com/libraries/microsoft-signalr)
* [jsDelivr](https://www.jsdelivr.com/package/npm/@microsoft/signalr)
* [unpkg](https://unpkg.com/@microsoft/signalr@next/dist/browser/signalr.min.js)

### Install with LibMan

[LibMan](xref:client-side/libman/index) can be used to install specific client library files from the CDN-hosted client library. For example, only add the minified JavaScript file to the project. For details on that approach, see [Add the SignalR client library](xref:tutorials/signalr#add-the-signalr-client-library).

## Connect to a hub

The following code creates and starts a connection. The hub's name is case insensitive:

[!code-javascript[](javascript-client/samples/3.x/SignalRChat/wwwroot/chat.js?range=3-6,29-43)]

### Cross-origin connections

Typically, browsers load connections from the same domain as the requested page. However, there are occasions when a connection to another domain is required.

> [!IMPORTANT]
> The client code must use an absolute URL instead of a relative URL. Change `.withUrl("/chathub")` to `.withUrl("https://myappurl/chathub")`.

To prevent a malicious site from reading sensitive data from another site, [cross-origin connections](xref:security/cors) are disabled by default. To allow a cross-origin request, enable it in the `Startup` class:

[!code-csharp[](javascript-client/samples/3.x/SignalRChat/Startup.cs?highlight=16-23,40)]

## Call hub methods from the client

JavaScript clients call public methods on hubs via the [invoke](/javascript/api/%40microsoft/signalr/hubconnection#invoke-string--any---) method of the [HubConnection](/javascript/api/%40microsoft/signalr/hubconnection). The `invoke` method accepts:

* The name of the hub method.
* Any arguments defined in the hub method.

In the following example, the method name on the hub is `SendMessage`. The second and third arguments passed to `invoke` map to the hub method's `user` and `message` arguments:

[!code-javascript[](javascript-client/samples/3.x/SignalRChat/wwwroot/chat.js?name=snippet_Invoke&highlight=2)]

> [!NOTE]
> Calling hub methods from a client is only supported when using the Azure SignalR Service in *Default* mode. For more information, see [Frequently Asked Questions (azure-signalr GitHub repository)](https://github.com/Azure/azure-signalr/blob/dev/docs/faq.md#what-is-the-meaning-of-service-mode-defaultserverlessclassic-how-can-i-choose).

The `invoke` method returns a JavaScript [Promise](https://developer.mozilla.org/docs/Web/JavaScript/Reference/Global_Objects/Promise). The `Promise` is resolved with the return value (if any) when the method on the server returns. If the method on the server throws an error, the `Promise` is rejected with the error message. Use `async` and `await` or the `Promise`'s `then` and `catch` methods to handle these cases.

JavaScript clients can also call public methods on hubs via the the [send](/javascript/api/%40microsoft/signalr/hubconnection#send-string--any---) method of the `HubConnection`. Unlike the `invoke` method, the `send` method doesn't wait for a response from the server. The `send` method returns a JavaScript `Promise`. The `Promise` is resolved when the message has been sent to the server. If there is an error sending the message, the `Promise` is rejected with the error message. Use `async` and `await` or the `Promise`'s `then` and `catch` methods to handle these cases.

> [!NOTE]
> Using `send` doesn't wait until the server has received the message. Consequently, it's not possible to return data or errors from the server.

## Call client methods from the hub

To receive messages from the hub, define a method using the [on](/javascript/api/%40microsoft/signalr/hubconnection#on-string---args--any-------void-) method of the `HubConnection`.

* The name of the JavaScript client method.
* Arguments the hub passes to the method.

In the following example, the method name is `ReceiveMessage`. The argument names are `user` and `message`:

[!code-javascript[](javascript-client/samples/3.x/SignalRChat/wwwroot/chat.js?name=snippet_ReceiveMessage)]

The preceding code in `connection.on` runs when server-side code calls it using the <xref:Microsoft.AspNetCore.SignalR.ClientProxyExtensions.SendAsync%2A> method:

[!code-csharp[Call client-side](javascript-client/samples/3.x/SignalRChat/Hubs/ChatHub.cs?name=snippet_SendMessage)]

SignalR determines which client method to call by matching the method name and arguments defined in `SendAsync` and `connection.on`.

> [!NOTE]
> As a best practice, call the [start](/javascript/api/%40aspnet/signalr/hubconnection#start) method on the `HubConnection` after `on`. Doing so ensures your handlers are registered before any messages are received.

## Error handling and logging

Use `try` and `catch` with `async` and `await` or the `Promise`'s `catch` method to handle client-side errors. Use `console.error` to output errors to the browser's console:

[!code-javascript[](javascript-client/samples/3.x/SignalRChat/wwwroot/chat.js?name=snippet_Invoke&highlight=1,3-5)]

Set up client-side log tracing by passing a logger and type of event to log when the connection is made. Messages are logged with the specified log level and higher. Available log levels are as follows:

* `signalR.LogLevel.Error`: Error messages. Logs `Error` messages only.
* `signalR.LogLevel.Warning`: Warning messages about potential errors. Logs `Warning`, and `Error` messages.
* `signalR.LogLevel.Information`: Status messages without errors. Logs `Information`, `Warning`, and `Error` messages.
* `signalR.LogLevel.Trace`: Trace messages. Logs everything, including data transported between hub and client.

Use the [configureLogging](/javascript/api/%40aspnet/signalr/hubconnectionbuilder#configurelogging) method on [HubConnectionBuilder](/javascript/api/%40aspnet/signalr/hubconnectionbuilder) to configure the log level. Messages are logged to the browser console:

[!code-javascript[](javascript-client/samples/3.x/SignalRChat/wwwroot/chat.js?name=snippet_Connection&highlight=3)]

## Reconnect clients

### Automatically reconnect

The JavaScript client for SignalR can be configured to automatically reconnect using the `withAutomaticReconnect` method on [HubConnectionBuilder](/javascript/api/%40aspnet/signalr/hubconnectionbuilder). It won't automatically reconnect by default.

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .withAutomaticReconnect()
    .build();
```

Without any parameters, `withAutomaticReconnect()` configures the client to wait 0, 2, 10, and 30 seconds respectively before trying each reconnect attempt, stopping after four failed attempts.

Before starting any reconnect attempts, the `HubConnection` will transition to the `HubConnectionState.Reconnecting` state and fire its `onreconnecting` callbacks instead of transitioning to the `Disconnected` state and triggering its `onclose` callbacks like a `HubConnection` without automatic reconnect configured. This provides an opportunity to warn users that the connection has been lost and to disable UI elements.

```javascript
connection.onreconnecting(error => {
    console.assert(connection.state === signalR.HubConnectionState.Reconnecting);

    document.getElementById("messageInput").disabled = true;

    const li = document.createElement("li");
    li.textContent = `Connection lost due to error "${error}". Reconnecting.`;
    document.getElementById("messagesList").appendChild(li);
});
```

If the client successfully reconnects within its first four attempts, the `HubConnection` will transition back to the `Connected` state and fire its `onreconnected` callbacks. This provides an opportunity to inform users the connection has been reestablished.

Since the connection looks entirely new to the server, a new `connectionId` will be provided to the `onreconnected` callback.

> [!WARNING]
> The `onreconnected` callback's `connectionId` parameter will be undefined if the `HubConnection` was configured to [skip negotiation](xref:signalr/configuration#configure-client-options).

```javascript
connection.onreconnected(connectionId => {
    console.assert(connection.state === signalR.HubConnectionState.Connected);

    document.getElementById("messageInput").disabled = false;

    const li = document.createElement("li");
    li.textContent = `Connection reestablished. Connected with connectionId "${connectionId}".`;
    document.getElementById("messagesList").appendChild(li);
});
```

`withAutomaticReconnect()` won't configure the `HubConnection` to retry initial start failures, so start failures need to be handled manually:

```javascript
async function start() {
    try {
        await connection.start();
        console.assert(connection.state === signalR.HubConnectionState.Connected);
        console.log("SignalR Connected.");
    } catch (err) {
        console.assert(connection.state === signalR.HubConnectionState.Disconnected);
        console.log(err);
        setTimeout(() => start(), 5000);
    }
};
```

If the client doesn't successfully reconnect within its first four attempts, the `HubConnection` will transition to the `Disconnected` state and fire its [onclose](/javascript/api/%40aspnet/signalr/hubconnection#onclose) callbacks. This provides an opportunity to inform users the connection has been permanently lost and recommend refreshing the page:

```javascript
connection.onclose(error => {
    console.assert(connection.state === signalR.HubConnectionState.Disconnected);

    document.getElementById("messageInput").disabled = true;

    const li = document.createElement("li");
    li.textContent = `Connection closed due to error "${error}". Try refreshing this page to restart the connection.`;
    document.getElementById("messagesList").appendChild(li);
});
```

In order to configure a custom number of reconnect attempts before disconnecting or change the reconnect timing, `withAutomaticReconnect` accepts an array of numbers representing the delay in milliseconds to wait before starting each reconnect attempt.

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .withAutomaticReconnect([0, 0, 10000])
    .build();

    // .withAutomaticReconnect([0, 2000, 10000, 30000]) yields the default behavior
```

The preceding example configures the `HubConnection` to start attempting reconnects immediately after the connection is lost. This is also true for the default configuration.

If the first reconnect attempt fails, the second reconnect attempt will also start immediately instead of waiting 2 seconds like it would in the default configuration.

If the second reconnect attempt fails, the third reconnect attempt will start in 10 seconds which is again like the default configuration.

The custom behavior then diverges again from the default behavior by stopping after the third reconnect attempt failure instead of trying one more reconnect attempt in another 30 seconds like it would in the default configuration.

If you want even more control over the timing and number of automatic reconnect attempts, `withAutomaticReconnect` accepts an object implementing the `IRetryPolicy` interface, which has a single method named `nextRetryDelayInMilliseconds`.

`nextRetryDelayInMilliseconds` takes a single argument with the type `RetryContext`. The `RetryContext` has three properties: `previousRetryCount`, `elapsedMilliseconds` and `retryReason` which are a `number`, a `number` and an `Error` respectively. Before the first reconnect attempt, both `previousRetryCount` and `elapsedMilliseconds` will be zero, and the `retryReason` will be the Error that caused the connection to be lost. After each failed retry attempt, `previousRetryCount` will be incremented by one, `elapsedMilliseconds` will be updated to reflect the amount of time spent reconnecting so far in milliseconds, and the `retryReason` will be the Error that caused the last reconnect attempt to fail.

`nextRetryDelayInMilliseconds` must return either a number representing the number of milliseconds to wait before the next reconnect attempt or `null` if the `HubConnection` should stop reconnecting.

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: retryContext => {
            if (retryContext.elapsedMilliseconds < 60000) {
                // If we've been reconnecting for less than 60 seconds so far,
                // wait between 0 and 10 seconds before the next reconnect attempt.
                return Math.random() * 10000;
            } else {
                // If we've been reconnecting for more than 60 seconds so far, stop reconnecting.
                return null;
            }
        }
    })
    .build();
```

Alternatively, you can write code that will reconnect your client manually as demonstrated in [Manually reconnect](#manually-reconnect).

### Manually reconnect

The following code demonstrates a typical manual reconnection approach:

1. A function (in this case, the `start` function) is created to start the connection.
1. Call the `start` function in the connection's `onclose` event handler.

[!code-javascript[](javascript-client/samples/3.x/SignalRChat/wwwroot/chat.js?range=30-46)]

A production implementation would use an exponential back-off or retry a specified number of times before giving up.

## Browser sleeping tab

Some browsers have a tab freezing or sleeping feature to reduce computer resource usage for inactive tabs. This can cause SignalR connections to close and may result in an unwanted user experience. Browsers use heuristics to figure out if a tab should be put to sleep, such as:

* Playing audio
* Holding a web lock
* Holding an `IndexedDB` lock
* Being connected to a USB device
* Capturing video or audio
* Being mirrored
* Capturing a window or display

> [!NOTE]
> These heuristics may change over time or differ between browsers. Check your support matrix and figure out what method works best for your scenarios.

To avoid putting an app to sleep, the app should trigger one of the heuristics that the browser uses.

The following code example shows how to use a [Web Lock](https://developer.mozilla.org/docs/Web/API/Web_Locks_API) to keep a tab awake and avoid an unexpected connection closure.

```javascript
var lockResolver;
if (navigator && navigator.locks && navigator.locks.request) {
    const promise = new Promise((res) => {
        lockResolver = res;
    });
    
    navigator.locks.request('unique_lock_name', { mode: "shared" }, () => {
        return promise;
    });
}
```

For the preceding code example:

* Web Locks are experimental. The conditional check confirms that the browser supports Web Locks.
* The promise resolver (`lockResolver`) is stored so that the lock can be released when it's acceptable for the tab to sleep.
* When closing the connection, the lock is released by calling `lockResolver()`. When the lock is released, the tab is allowed to sleep.

## Additional resources

* [JavaScript API reference](/javascript/api/?view=signalr-js-latest&preserve-view=true )
* [JavaScript tutorial](xref:tutorials/signalr)
* [WebPack and TypeScript tutorial](xref:tutorials/signalr-typescript-webpack)
* [Hubs](xref:signalr/hubs)
* [.NET client](xref:signalr/dotnet-client)
* [Publish to Azure](xref:signalr/publish-to-azure-web-app)
* [Cross-Origin Requests (CORS)](xref:security/cors)
* [Azure SignalR Service serverless documentation](/azure/azure-signalr/signalr-concept-serverless-development-config)
* [Troubleshoot connection errors](xref:signalr/troubleshoot)

::: moniker-end

::: moniker range="< aspnetcore-3.0"

By [Rachel Appel](https://twitter.com/rachelappel)

The ASP.NET Core SignalR JavaScript client library enables developers to call server-side hub code.

[View or download sample code](https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/signalr/javascript-client/samples) ([how to download](xref:index#how-to-download-a-sample))

## Install the SignalR client package

The SignalR JavaScript client library is delivered as an [npm](https://www.npmjs.com/) package. The following sections outline different ways to install the client library.

### Install with npm

If using Visual Studio, run the following commands from **Package Manager Console** while in the root folder. For Visual Studio Code, run the following commands from the **Integrated Terminal**.

```bash
npm init -y
npm install @aspnet/signalr
```

npm installs the package contents in the *node_modules\\@aspnet\signalr\dist\browser* folder. Create a new folder named *signalr* under the *wwwroot\\lib* folder. Copy the *signalr.js* file to the *wwwroot\lib\signalr* folder.

Reference the SignalR JavaScript client in the `<script>` element. For example:

```html
<script src="~/lib/signalr/signalr.js"></script>
```

### Use a Content Delivery Network (CDN)

To use the client library without the npm prerequisite, reference a CDN-hosted copy of the client library. For example:

```html
<script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/3.1.3/signalr.min.js"></script>
```

The client library is available on the following CDNs:

* [cdnjs](https://cdnjs.com/libraries/aspnet-signalr)
* [jsDelivr](https://www.jsdelivr.com/package/npm/@aspnet/signalr)
* [unpkg](https://unpkg.com/@aspnet/signalr@next/dist/browser/signalr.min.js)

### Install with LibMan

[LibMan](xref:client-side/libman/index) can be used to install specific client library files from the CDN-hosted client library. For example, only add the minified JavaScript file to the project. For details on that approach, see [Add the SignalR client library](xref:tutorials/signalr#add-the-signalr-client-library).

## Connect to a hub

The following code creates and starts a connection. The hub's name is case insensitive.

[!code-javascript[Call hub methods](javascript-client/samples/2.x/SignalRChat/wwwroot/js/chat.js?range=9-13,28-51)]

### Cross-origin connections

Typically, browsers load connections from the same domain as the requested page. However, there are occasions when a connection to another domain is required.

To prevent a malicious site from reading sensitive data from another site, [cross-origin connections](xref:security/cors) are disabled by default. To allow a cross-origin request, enable it in the `Startup` class.

[!code-csharp[Cross-origin connections](javascript-client/samples/2.x/SignalRChat/Startup.cs?highlight=29-35,56)]

## Call hub methods from client

JavaScript clients call public methods on hubs via the [invoke](/javascript/api/%40aspnet/signalr/hubconnection#invoke) method of the [HubConnection](/javascript/api/%40aspnet/signalr/hubconnection). The `invoke` method accepts two arguments:

* The name of the hub method. In the following example, the method name on the hub is `SendMessage`.
* Any arguments defined in the hub method. In the following example, the argument name is `message`. The example code uses arrow function syntax that is supported in current versions of all major browsers except Internet Explorer.

  [!code-javascript[Call hub methods](javascript-client/samples/2.x/SignalRChat/wwwroot/js/chat.js?range=24)]

> [!NOTE]
> Calling hub methods from a client is only supported when using the Azure SignalR Service in *Default* mode. For more information, see [Frequently Asked Questions (azure-signalr GitHub repository)](https://github.com/Azure/azure-signalr/blob/dev/docs/faq.md#what-is-the-meaning-of-service-mode-defaultserverlessclassic-how-can-i-choose).

The `invoke` method returns a JavaScript [Promise](https://developer.mozilla.org/docs/Web/JavaScript/Reference/Global_Objects/Promise). The `Promise` is resolved with the return value (if any) when the method on the server returns. If the method on the server throws an error, the `Promise` is rejected with the error message. Use the `then` and `catch` methods on the `Promise` itself to handle these cases (or `await` syntax).

The `send` method returns a JavaScript `Promise`. The `Promise` is resolved when the message has been sent to the server. If there is an error sending the message, the `Promise` is rejected with the error message. Use the `then` and `catch` methods on the `Promise` itself to handle these cases (or `await` syntax).

> [!NOTE]
> Using `send` doesn't wait until the server has received the message. Consequently, it's not possible to return data or errors from the server.

## Call client methods from hub

To receive messages from the hub, define a method using the [on](/javascript/api/%40aspnet/signalr/hubconnection#on) method of the `HubConnection`.

* The name of the JavaScript client method. In the following example, the method name is `ReceiveMessage`.
* Arguments the hub passes to the method. In the following example, the argument value is `message`.

[!code-javascript[Receive calls from hub](javascript-client/samples/2.x/SignalRChat/wwwroot/js/chat.js?range=14-19)]

The preceding code in `connection.on` runs when server-side code calls it using the <xref:Microsoft.AspNetCore.SignalR.ClientProxyExtensions.SendAsync%2A> method.

[!code-csharp[Call client-side](javascript-client/samples/2.x/SignalRChat/hubs/chathub.cs?range=8-11)]

SignalR determines which client method to call by matching the method name and arguments defined in `SendAsync` and `connection.on`.

> [!NOTE]
> As a best practice, call the [start](/javascript/api/%40aspnet/signalr/hubconnection#start) method on the `HubConnection` after `on`. Doing so ensures your handlers are registered before any messages are received.

## Error handling and logging

Chain a `catch` method to the end of the `start` method to handle client-side errors. Use `console.error` to output errors to the browser's console.

[!code-javascript[Error handling](javascript-client/samples/2.x/SignalRChat/wwwroot/js/chat.js?range=50)]

Set up client-side log tracing by passing a logger and type of event to log when the connection is made. Messages are logged with the specified log level and higher. Available log levels are as follows:

* `signalR.LogLevel.Error`: Error messages. Logs `Error` messages only.
* `signalR.LogLevel.Warning`: Warning messages about potential errors. Logs `Warning`, and `Error` messages.
* `signalR.LogLevel.Information`: Status messages without errors. Logs `Information`, `Warning`, and `Error` messages.
* `signalR.LogLevel.Trace`: Trace messages. Logs everything, including data transported between hub and client.

Use the [configureLogging](/javascript/api/%40aspnet/signalr/hubconnectionbuilder#configurelogging) method on [HubConnectionBuilder](/javascript/api/%40aspnet/signalr/hubconnectionbuilder) to configure the log level. Messages are logged to the browser console.

[!code-javascript[Logging levels](javascript-client/samples/2.x/SignalRChat/wwwroot/js/chat.js?range=9-12)]

## Reconnect clients

### Manually reconnect

> [!WARNING]
> Prior to 3.0, the JavaScript client for SignalR doesn't automatically reconnect. You must write code that will reconnect your client manually.

The following code demonstrates a typical manual reconnection approach:

1. A function (in this case, the `start` function) is created to start the connection.
1. Call the `start` function in the connection's `onclose` event handler.

[!code-javascript[Reconnect the JavaScript client](javascript-client/samples/2.x/SignalRChat/wwwroot/js/chat.js?range=28-40)]

A real-world implementation would use an exponential back-off or retry a specified number of times before giving up.

## Additional resources

* [JavaScript API reference](/javascript/api/?view=signalr-js-latest)
* [JavaScript tutorial](xref:tutorials/signalr)
* [WebPack and TypeScript tutorial](xref:tutorials/signalr-typescript-webpack)
* [Hubs](xref:signalr/hubs)
* [.NET client](xref:signalr/dotnet-client)
* [Publish to Azure](xref:signalr/publish-to-azure-web-app)
* [Cross-Origin Requests (CORS)](xref:security/cors)
* [Azure SignalR Service serverless documentation](/azure/azure-signalr/signalr-concept-serverless-development-config)

::: moniker-end
