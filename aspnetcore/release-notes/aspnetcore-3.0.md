---
title: What's new in ASP.NET Core 3.0
author: rick-anderson
description: Learn about the new features in ASP.NET Core 3.0.
ms.author: riande
ms.custom: mvc
ms.date: 12/18/2018
uid: aspnetcore-3.0
---
# What's new in ASP.NET Core 3.0

This article highlights the most significant changes in ASP.NET Core 3.0, with links to relevant documentation.

## ASP.NET Core 3.0 only runs on .NET Core 3.0

Apps using ASP.NET Core on .NET Framework can continue in a fully supported fashion using the [2.1 LTS release](https://www.microsoft.com/net/download/dotnet-core/2.1). ASP.NET Core 2.1 is supported until August 21, 2021. For more information, see [this GitHub announcement](https://github.com/aspnet/Announcements/issues/324).

See [Port your code from .NET Framework to .NET Core](/dotnet/core/porting/) for migration information.

## Microsoft.AspNetCore.App

The [ASP.NET Core 3.0 shared framework](https://natemcmaster.com/blog/2018/08/29/netcore-primitives-2/) no longer contains:

* [Newtonsoft.Json Json.NET](https://www.nuget.org/packages/Newtonsoft.Json/). To add Json.NET to ASP.NET Core 3.0, see <!-- This link won't work until 3.0 GA's, that is 3.0 is the default version for docs. --> [Add Newtonsoft.Json-based JSON format support](xref:web-api/advanced/formatting#add-newtonsoftjson-based-json-format-support). <!-- hard coded link [Add Newtonsoft.Json-based JSON format support](https://docs.microsoft.com/aspnet/core/web-api/advanced/formatting?view=aspnetcore-3.0#add-newtonsoftjson-based-json-format-support). -->
* [Entity Framework Core](/ef/core/)

The ASP.NET Core 3.0 shared framework is contained in the [`Microsoft.AspNetCore.App`](xref:fundamentals/metapackage-app) metapackage. For more information, see [this GitHub issue](https://github.com/aspnet/Announcements/issues/325). For more information on the motivation for this change, see [this blog](https://devblogs.microsoft.com/aspnet/a-first-look-at-changes-coming-in-asp-net-core-3-0/).

## Razor Components

@guardrex to provide

## SignalR

@bradygaster  to provide

## Generic host

The ASP.NET Core 3.0 templates use <xref:fundamentals/host/generic-host>. Previous versions used <xref:Microsoft.AspNetCore.Hosting.WebHostBuilder>. Using the .NET Core Generic Host (<xref:Microsoft.Extensions.Hosting.HostBuilder>) provides better integration of ASP.NET Core apps with other server scenarios that are not web specific.

The following two images show the changes made to the template generated *Program.cs* file:

 ![2.2 version](aspnetcore-3.0/_static/2.2host.png)

 ![3.0 version](aspnetcore-3.0/_static/3.0host.png)

In the preceding images:

* The ASP.NET Core 2.2 version is shown first. The red boxes indicate code that has been removed.
* The ASP.NET Core 3.0 version is shown second. The green boxes indicate code that has been added.




The first image is the ASP.NET Core 2.2 version. 

## Additional information

<!-- 
For the complete list of changes, see the [ASP.NET Core 2.2 Release Notes](https://github.com/aspnet/Home/releases/tag/2.2.0).
-->
