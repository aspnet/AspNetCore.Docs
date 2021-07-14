---
title: Host ASP.NET Core in Docker containers
author: rick-anderson
description: Discover links to resources for learning how to host ASP.NET Core apps in Docker containers.
ms.author: riande
ms.custom: mvc
ms.date: 01/08/2018
no-loc: [Home, Privacy, Kestrel, appsettings.json, "ASP.NET Core Identity", cookie, Cookie, Blazor, "Blazor Server", "Blazor WebAssembly", "Identity", "Let's Encrypt", Razor, SignalR]
uid: host-and-deploy/docker/index
---
# Host ASP.NET Core in Docker containers

The following articles are available for learning about hosting ASP.NET Core apps in Docker:

[Introduction to Containers and Docker](/dotnet/standard/microservices-architecture/container-docker-introduction/index)  
See how containerization is an approach to software development in which an application or service, its dependencies, and its configuration are packaged together as a container image. The image can be tested and then deployed to a host.

[What is Docker](/dotnet/standard/microservices-architecture/container-docker-introduction/docker-defined)  
Discover how Docker is an open-source project for automating the deployment of apps as portable, self-sufficient containers that can run on the cloud or on-premises.

[Docker Terminology](/dotnet/standard/microservices-architecture/container-docker-introduction/docker-terminology)  
Learn terms and definitions for Docker technology.

[Docker containers, images, and registries](/dotnet/standard/microservices-architecture/container-docker-introduction/docker-containers-images-registries)  
Find out how Docker container images are stored in an image registry for consistent deployment across environments.

<xref:host-and-deploy/docker/building-net-docker-images>
Learn how to build and dockerize an ASP.NET Core app. Explore Docker images maintained by Microsoft and examine use cases.

[Visual Studio Container Tools](xref:host-and-deploy/docker/visual-studio-tools-for-docker)  
Discover how Visual Studio supports building, debugging, and running ASP.NET Core apps targeting either .NET Framework or .NET Core on Docker for Windows. Both Windows and Linux containers are supported.

[Publish to Azure Container Registry](/azure/vs-azure-tools-docker-hosting-web-apps-in-docker)  
Find out how to use the Visual Studio Container Tools extension to deploy an ASP.NET Core app to a Docker host on Azure using PowerShell.

[Configure ASP.NET Core to work with proxy servers and load balancers](xref:host-and-deploy/proxy-load-balancer)  
Additional configuration might be required for apps hosted behind proxy servers and load balancers. Passing requests through a proxy often obscures information about the original request, such as the scheme and client IP. It might be necessary to forwarded some information about the request manually to the app.

[GC using Docker and small containers](xref:performance/memory#sc)
Discusses GC selection with small containers.

[!INCLUDE[](~/includes/docker-128.md)]