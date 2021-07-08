---
title: ASP.NET Core load/stress testing
author: Jeremy-Meng
description: Learn about several notable tools and approaches for load testing and stress testing ASP.NET Core apps.
ms.author: riande
ms.custom: mvc
ms.date: 4/05/2019
no-loc: [Home, Privacy, Kestrel, appsettings.json, "ASP.NET Core Identity", cookie, Cookie, Blazor, "Blazor Server", "Blazor WebAssembly", "Identity", "Let's Encrypt", Razor, SignalR]
uid: test/loadtests
---
# ASP.NET Core load/stress testing

Load testing and stress testing are important to ensure a web app is performant and scalable. Their goals are different even though they often share similar tests.

**Load tests**: Test whether the app can handle a specified load of users for a certain scenario while still satisfying the response goal. The app is run under normal conditions.

**Stress tests**: Test app stability when running under extreme conditions, often for a long period of time. The tests place high user load, either spikes or gradually increasing load, on the app, or they limit the app's computing resources.

Stress tests determine if an app under stress can recover from failure and gracefully return to expected behavior. Under stress, the app isn't run under normal conditions.

Visual Studio 2019 announced plans to [deprecate the load testing](https://devblogs.microsoft.com/devops/cloud-based-load-testing-service-eol/). The corresponding Azure DevOps cloud-based load testing service has been closed.

## Third-party tools

The following list contains third-party web performance tools with various feature sets:

* [Apache JMeter](https://jmeter.apache.org/)
* [ApacheBench (ab)](https://httpd.apache.org/docs/2.4/programs/ab.html)
* [Gatling](https://gatling.io/)
* [k6](https://k6.io)
* [Locust](https://locust.io/)
* [West Wind WebSurge](https://websurge.west-wind.com/)
* [Netling](https://github.com/hallatore/Netling)
* [Vegeta](https://github.com/tsenart/vegeta)
* [NBomber](https://github.com/PragmaticFlow/NBomber)

## Load and stress test with release builds

Load and stress tests should be done in release and [production](xref:fundamentals/environments) mode and not in debug and development mode. [Release configurations](/visualstudio/debugger/how-to-set-debug-and-release-configurations) are fully optimized with minimal logging. Debug configuration is not optimized.  [Development](xref:fundamentals/environments) mode enables more information logging that can impact performance.
