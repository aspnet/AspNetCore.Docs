---
title: Docker images for ASP.NET Core
author: rick-anderson
description: Learn how to use the published ASP.NET Core Docker images from the Docker Registry. Pull and build your own images.
ms.author: riande
ms.custom: mvc
ms.date: 01/04/2021
no-loc: [Home, Privacy, Kestrel, appsettings.json, "ASP.NET Core Identity", cookie, Cookie, Blazor, "Blazor Server", "Blazor WebAssembly", "Identity", "Let's Encrypt", Razor, SignalR]
uid: host-and-deploy/docker/building-net-docker-images
---

# Docker images for ASP.NET Core

This tutorial shows how to run an ASP.NET Core app in Docker containers.

In this tutorial, you:
> [!div class="checklist"]
> * Learn about ASP.NET Core Docker images
> * Download an ASP.NET Core sample app
> * Run the sample app locally
> * Run the sample app in Linux containers
> * Run the sample app in Windows containers
> * Build and deploy manually

## ASP.NET Core Docker images

For this tutorial, you download an ASP.NET Core sample app and run it in Docker containers. The sample works with both Linux and Windows containers.

The sample Dockerfile uses the [Docker multi-stage build feature](https://docs.docker.com/engine/userguide/eng-image/multistage-build/) to build and run in different containers. The build and run containers are created from images that are provided in Docker Hub by Microsoft:

::: moniker range=">= aspnetcore-5.0"

* `dotnet/sdk`

  The sample uses this image for building the app. The image contains the .NET SDK, which includes the Command Line Tools (CLI). The image is optimized for local development, debugging, and unit testing. The tools installed for development and compilation make the image relatively large.

::: moniker-end

::: moniker range="< aspnetcore-5.0"

* `dotnet/core/sdk`

  The sample uses this image for building the app. The image contains the .NET Core SDK, which includes the Command Line Tools (CLI). The image is optimized for local development, debugging, and unit testing. The tools installed for development and compilation make the image relatively large.

::: moniker-end

::: moniker range=">= aspnetcore-5.0"

* `dotnet/aspnet`

   The sample uses this image for running the app. The image contains the ASP.NET Core runtime and libraries and is optimized for running apps in production. Designed for speed of deployment and app startup, the image is relatively small, so network performance from Docker Registry to Docker host is optimized. Only the binaries and content needed to run an app are copied to the container. The contents are ready to run, enabling the fastest time from `docker run` to app startup. Dynamic code compilation isn't needed in the Docker model.
   
::: moniker-end

::: moniker range="< aspnetcore-5.0"

* `dotnet/core/aspnet`

   The sample uses this image for running the app. The image contains the ASP.NET Core runtime and libraries and is optimized for running apps in production. Designed for speed of deployment and app startup, the image is relatively small, so network performance from Docker Registry to Docker host is optimized. Only the binaries and content needed to run an app are copied to the container. The contents are ready to run, enabling the fastest time from `docker run` to app startup. Dynamic code compilation isn't needed in the Docker model.
   
::: moniker-end

## Prerequisites

::: moniker range=">= aspnetcore-5.0"

* [.NET SDK 5.0](https://dotnet.microsoft.com/download)

::: moniker-end

::: moniker range=">= aspnetcore-3.0 < aspnetcore-5.0"

* [.NET Core SDK 3.1](https://dotnet.microsoft.com/download)

::: moniker-end

::: moniker range="< aspnetcore-3.0"

* [.NET Core 2.2 SDK](https://dotnet.microsoft.com/download/dotnet-core)

::: moniker-end

* Docker client 18.03 or later

  * Linux distributions
    * [CentOS](https://docs.docker.com/install/linux/docker-ce/centos/)
    * [Debian](https://docs.docker.com/install/linux/docker-ce/debian/)
    * [Fedora](https://docs.docker.com/install/linux/docker-ce/fedora/)
    * [Ubuntu](https://docs.docker.com/install/linux/docker-ce/ubuntu/)
  * [macOS](https://docs.docker.com/docker-for-mac/install/)
  * [Windows](https://docs.docker.com/docker-for-windows/install/)

* [Git](https://git-scm.com/download)

## Download the sample app

* Download the sample by cloning the [.NET Docker repository](https://github.com/dotnet/dotnet-docker): 

  ```console
  git clone https://github.com/dotnet/dotnet-docker
  ```

## Run the app locally

* Navigate to the project folder at *dotnet-docker/samples/aspnetapp/aspnetapp*.

* Run the following command to build and run the app locally:

  ```dotnetcli
  dotnet run
  ```

* Go to `http://localhost:5000` in a browser to test the app.

* Press Ctrl+C at the command prompt to stop the app.

## Run in a Linux container

* In the Docker client, [switch to Linux containers](https://docs.docker.com/docker-for-windows/#switch-between-windows-and-linux-containers).

* Navigate to the Dockerfile folder at *dotnet-docker/samples/aspnetapp*.

* Run the following commands to build and run the sample in Docker:

  ```console
  docker build -t aspnetapp .
  docker run -it --rm -p 5000:80 --name aspnetcore_sample aspnetapp
  ```

  The `build` command arguments:
  * Name the image aspnetapp.
  * Look for the Dockerfile in the current folder (the period at the end).

  The run command arguments:
  * Allocate a pseudo-TTY and keep it open even if not attached. (Same effect as `--interactive --tty`.)
  * Automatically remove the container when it exits.
  * Map port 5000 on the local machine to port 80 in the container.
  * Name the container aspnetcore_sample.
  * Specify the aspnetapp image.

* Go to `http://localhost:5000` in a browser to test the app.

## Run in a Windows container

* In the Docker client, [switch to Windows containers](https://docs.docker.com/docker-for-windows/#switch-between-windows-and-linux-containers).

Navigate to the docker file folder at `dotnet-docker/samples/aspnetapp`.

* Run the following commands to build and run the sample in Docker:

  ```console
  docker build -t aspnetapp .
  docker run -it --rm --name aspnetcore_sample aspnetapp
  ```

* For Windows containers, you need the IP address of the container (browsing to `http://localhost:5000` won't work):
  * Open up another command prompt.
  * Run `docker ps` to see the running containers. Verify that the "aspnetcore_sample" container is there.
  * Run `docker exec aspnetcore_sample ipconfig` to display the IP address of the container. The output from the command looks like this example:

    ```console
    Ethernet adapter Ethernet:

       Connection-specific DNS Suffix  . : contoso.com
       Link-local IPv6 Address . . . . . : fe80::1967:6598:124:cfa3%4
       IPv4 Address. . . . . . . . . . . : 172.29.245.43
       Subnet Mask . . . . . . . . . . . : 255.255.240.0
       Default Gateway . . . . . . . . . : 172.29.240.1
    ```

* Copy the container IPv4 address (for example, 172.29.245.43) and paste into the browser address bar to test the app.

## Build and deploy manually

In some scenarios, you might want to deploy an app to a container by copying its assets that are needed at run time. This section shows how to deploy manually.

* Navigate to the project folder at *dotnet-docker/samples/aspnetapp/aspnetapp*.

* Run the [dotnet publish](/dotnet/core/tools/dotnet-publish) command:

  ```dotnetcli
  dotnet publish -c Release -o published
  ```

  The command arguments:
  * Build the app in release mode (the default is debug mode).
  * Create the assets in the *published* folder.

* Run the app.

  * Windows:

    ```dotnetcli
    dotnet published\aspnetapp.dll
    ```

  * Linux:

    ```dotnetcli
    dotnet published/aspnetapp.dll
    ```

* Browse to `http://localhost:5000` to see the home page.

To use the manually published app within a Docker container, create a new *Dockerfile* and use the `docker build .` command to build an image.

::: moniker range=">= aspnetcore-5.0"

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY published/aspnetapp.dll ./
ENTRYPOINT ["dotnet", "aspnetapp.dll"]
```

To see the new image use the `docker images` command.

### The Dockerfile

Here's the *Dockerfile* used by the `docker build` command you ran earlier.  It uses `dotnet publish` the same way you did in this section to build and deploy.  

```dockerfile
# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY aspnetapp/*.csproj ./aspnetapp/
RUN dotnet restore

# copy everything else and build app
COPY aspnetapp/. ./aspnetapp/
WORKDIR /source/aspnetapp
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "aspnetapp.dll"]
```

In the preceding *Dockerfile*, the `*.csproj` files are copied and restored as distinct *layers*. When the `docker build` command builds an image, it uses a built-in cache. If the `*.csproj` files haven't changed since the `docker build` command last ran, the `dotnet restore` command doesn't need to run again. Instead, the built-in cache for the corresponding `dotnet restore` layer is reused. For more information, see [Best practices for writing Dockerfiles](https://docs.docker.com/develop/develop-images/dockerfile_best-practices/#leverage-build-cache).

::: moniker-end

::: moniker range=">= aspnetcore-3.0 < aspnetcore-5.0"

```dockerfile
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
COPY published/aspnetapp.dll ./
ENTRYPOINT ["dotnet", "aspnetapp.dll"]
```

### The Dockerfile

Here's the *Dockerfile* used by the `docker build` command you ran earlier.  It uses `dotnet publish` the same way you did in this section to build and deploy.  

```dockerfile
FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY aspnetapp/*.csproj ./aspnetapp/
RUN dotnet restore

# copy everything else and build app
COPY aspnetapp/. ./aspnetapp/
WORKDIR /app/aspnetapp
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
COPY --from=build /app/aspnetapp/out ./
ENTRYPOINT ["dotnet", "aspnetapp.dll"]
```

As noted in the preceding Dockerfile, the `*.csproj` files are copied and restored as distinct *layers*. When the `docker build` command builds an image, it uses a built-in cache. If the `*.csproj` files haven't changed since the `docker build` command last ran, the `dotnet restore` command doesn't need to run again. Instead, the built-in cache for the corresponding `dotnet restore` layer is reused. For more information, see [Best practices for writing Dockerfiles](https://docs.docker.com/develop/develop-images/dockerfile_best-practices/#leverage-build-cache).

::: moniker-end

::: moniker range="< aspnetcore-3.0"

```dockerfile
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY published/aspnetapp.dll ./
ENTRYPOINT ["dotnet", "aspnetapp.dll"]
```

### The Dockerfile

Here's the *Dockerfile* used by the `docker build` command you ran earlier. It uses `dotnet publish` the same way you did in this section to build and deploy.  

```dockerfile
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY aspnetapp/*.csproj ./aspnetapp/
RUN dotnet restore

# copy everything else and build app
COPY aspnetapp/. ./aspnetapp/
WORKDIR /app/aspnetapp
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/aspnetapp/out ./
ENTRYPOINT ["dotnet", "aspnetapp.dll"]
```

::: moniker-end

## Additional resources

* [Docker build command](https://docs.docker.com/engine/reference/commandline/build)
* [Docker run command](https://docs.docker.com/engine/reference/commandline/run)
* [ASP.NET Core Docker sample](https://github.com/dotnet/dotnet-docker) (The one used in this tutorial.)
* [Configure ASP.NET Core to work with proxy servers and load balancers](../proxy-load-balancer.md)
* [Working with Visual Studio Docker Tools](./visual-studio-tools-for-docker.md)
* [Debugging with Visual Studio Code](https://code.visualstudio.com/docs/nodejs/debugging-recipes#_debug-nodejs-in-docker-containers)
* [GC using Docker and small containers](xref:performance/memory#sc)
* [System.IO.IOException: The configured user limit (128) on the number of inotify instances has been reached](xref:aspnet/core/host-and-deploy/docker#d128)

## Next steps

The Git repository that contains the sample app also includes documentation. For an overview of the resources available in the repository, see [the README file](https://github.com/dotnet/dotnet-docker/blob/main/samples/aspnetapp/README.md). In particular, learn how to implement HTTPS:

> [!div class="nextstepaction"]
> [Developing ASP.NET Core Applications with Docker over HTTPS](https://github.com/dotnet/dotnet-docker/blob/main/samples/run-aspnetcore-https-development.md)
