---
title: "Get started with Razor Pages in ASP.NET Core"
author: rick-anderson
monikerRange: '>= aspnetcore-2.1'
description: This series of tutorials shows how to use Razor Pages in ASP.NET Core. Learn how to create a model, generate code for Razor pages, use Entity Framework Core and SQL Server for data access, add search functionality, add input validation, and use migrations to update the model.
ms.author: riande
ms.date: 05/30/2018
uid: tutorials/razor-pages/razor-pages-start
---
# Tutorial: Get started with Razor Pages in ASP.NET Core

By [Rick Anderson](https://twitter.com/RickAndMSFT)

::: moniker range="= aspnetcore-2.2"

::: moniker-end

::: moniker range="= aspnetcore-2.1"

::: moniker-end

This tutorial teaches the basics of building an ASP.NET Core Razor Pages web app.

The app manages a database of movie titles. You learn how to:

> [!div class="checklist"]
> * Create a Razor Pages web app.
> * Add and scaffold a model.
> * Work with a database.
> * Add search and validation.

At the end, you have an app that can manage and display movie titles items.

[View or download sample code](https://github.com/aspnet/Docs/tree/master/aspnetcore/tutorials/razor-pages/razor-pages-start/sample) ([how to download](xref:index#how-to-download-a-sample)).

## Prerequisites

::: moniker range="= aspnetcore-2.2"
[!INCLUDE[](~/includes/net-core-prereqs-all-2.2.md)]

::: moniker-end
::: moniker range="= aspnetcore-2.1"
[!INCLUDE[](~/includes/net-core-prereqs-all-2.1.md)]

::: moniker-end

## Create a Razor web app

# [Visual Studio](#tab/visual-studio)

* From the Visual Studio **File** menu, select **New** > **Project**.
* Create a new ASP.NET Core Web Application. Name the project **RazorPagesMovie**. It's important to name the project *RazorPagesMovie* so the namespaces will match when you copy/paste code.
 ![new ASP.NET Core Web Application](razor-pages-start/_static/np_2.1.png)

::: moniker range="= aspnetcore-2.2"

* Select **ASP.NET Core 2.2** in the dropdown, and then select **Web Application**.

  ![new ASP.NET Core Web Application](razor-pages-start/_static/np_2_2.2.png)

  The following starter project is created:

  ![Solution Explorer](razor-pages-start/_static/se2.2.png)

::: moniker-end
::: moniker range="= aspnetcore-2.1"

* Select **ASP.NET Core 2.1** in the dropdown, and then select **Web Application**.

  ![new ASP.NET Core Web Application](razor-pages-start/_static/np_2_2.1.png)

  The Visual Studio template creates a starter project:

  ![Solution Explorer](razor-pages-start/_static/se2.1.png)

::: moniker-end

* Press **Ctrl-F5** to run without the debugger.

  Visual Studio starts [IIS Express](/iis/extensions/introduction-to-iis-express/iis-express-overview) and runs the app. The address bar shows `localhost:port#` and not something like `example.com`. That's because `localhost` is the standard hostname for your local computer. Localhost only serves web requests from the local computer. When Visual Studio creates a web project, a random port is used for the web server. In the preceding image, the port number is 5001. When you run the app, you'll see a different port number.

  Launching the app with **Ctrl+F5** (non-debug mode) allows you to make code changes, save the file, refresh the browser, and see the code changes. Many developers prefer to use non-debug mode to refresh the page and view changes.

# [Visual Studio Code](#tab/visual-studio-code)

* Open the [integrated terminal](https://code.visualstudio.com/docs/editor/integrated-terminal).
* Change directories (`cd`) to a folder which will contain the project.
* Run the following command:

   ```console
   dotnet new webapp -o RazorPagesMovie
   code -r RazorPagesMovie
   ```

  * A dialog box appears with **Required assets to build and debug are missing from 'RazorPagesMovie'. Add them?**
  * Select **Yes**
  * `dotnet new webapp -o RazorPagesMovie`: creates a new Web API project in the *RazorPagesMovie* folder.
  * `code -r RazorPagesMovie`: Loads the *RazorPagesMovie.csproj* project file.

### Launch the app

* Press **Ctrl-F5** to run without the debugger.

  Visual Studio Code starts starts [Kestrel](xref:fundamentals/servers/kestrel), launches a browser, and navigates to `http://localhost:5001`. The address bar shows `localhost:port:5001` and not something like `example.com`. That's because `localhost` is the standard hostname for your local computer. Localhost only serves web requests from the local computer.

  Launching the app with **Ctrl+F5** (non-debug mode) allows you to make code changes, save the file, refresh the browser, and see the code changes. Many developers prefer to use non-debug mode to refresh the page and view changes.

# [Visual Studio for Mac](#tab/visual-studio-mac)

From a terminal, run the following commands:

<!-- TODO: update these instruction once mac support 2.2 projects -->

::: moniker range="= aspnetcore-2.2"

```console
dotnet new webapp -o RazorPagesMovie
cd RazorPagesMovie
dotnet run
```

::: moniker-end

::: moniker range="= aspnetcore-2.1"

```console
dotnet new webapp -o RazorPagesMovie
cd RazorPagesMovie
dotnet run
```

::: moniker-end

The preceding commands use the [.NET Core CLI](/dotnet/core/tools/dotnet) to create and run a Razor Pages project. Open a browser to http://localhost:5000 to view the application.

## Open the project

Press Ctrl+C to shut down the application.

From Visual Studio, select **File > Open**, and then select the *RazorPagesMovie.csproj* file.

### Launch the app

Select **Run > Start Without Debugging** to launch the app. Visual Studio starts [Kestrel](xref:fundamentals/servers/kestrel), launches a browser, and navigates to `http://localhost:5001`.

<!-- End of VS tabs -->

---

* Select **Accept** to consent to tracking. This app doesn't track personal information. The template generated code includes assets to help meet [General Data Protection Regulation (GDPR)](xref:security/gdpr).

::: moniker range="= aspnetcore-2.2"
  ![Home or Index page](razor-pages-start/_static/homeGDPR2.2.png)

  The following image shows the app after accepting tracking:

  ![Home or Index page](razor-pages-start/_static/home2.2.png)
::: moniker-end
::: moniker range="= aspnetcore-2.1"
  ![Home or Index page](razor-pages-start/_static/homeGDPR.png)

  The following image shows the app after accepting tracking:

  ![Home or Index page](razor-pages-start/_static/home2.1.png)
::: moniker-end

::: moniker range="= aspnetcore-2.2"

The default template creates **RazorPagesMovie**, **Home**, and **Privacy** links and pages. Depending on the size of your browser window, you might need to click the navigation icon to show the links.

![Home or Index page](razor-pages-start/_static/xhome2.2.png)

Test the links. The **RazorPagesMovie** and **Home** links go to the Index page. The **Privacy** link go to the `Privacy` pages. For more information on the privacy page, see [General Data Protection Regulation (GDPR)](xref:security/gdpr).

## Project files and folders

The following table lists the files and folders in the project. At this point in the tutorial, the *Startup.cs* file is the most important to understand. You don't need to review each link provided below. The links are provided as a reference when you need more information on a file or folder in the project.

| File or folder              | Purpose |
| ----------------- | ------------ |
| *wwwroot* | Contains static files. See [Static files](xref:fundamentals/static-files). |
| *Pages* | Folder for [Razor Pages](xref:razor-pages/index). |
| *appsettings.json* | [Configuration](xref:fundamentals/configuration/index) |
| *Program.cs* | [Hosts](xref:fundamentals/host/index) the ASP.NET Core app.|
| *Startup.cs* | Configures services and the request pipeline. See [Startup](xref:fundamentals/startup).|

### The Pages folder

The *_Layout.cshtml* file contains common HTML elements (scripts and stylesheets) and sets the layout for the application. For example, when you click on **RazorPagesMovie**, **Home**, or **Privacy**, you see the same elements. The common elements include the navigation menu on the top and the header on the bottom of the window. See [Layout](xref:mvc/views/layout) for more information.

The *_ViewImports.cshtml* file contains Razor directives that are imported into each Razor Page. See [Importing Shared Directives](xref:mvc/views/layout#importing-shared-directives) for more information.

The *_ViewStart.cshtml* sets the Razor Pages `Layout` property to use the *_Layout.cshtml* file. See [Layout](xref:mvc/views/layout) for more information.

The *_ValidationScriptsPartial.cshtml* file provides a reference to [jQuery](https://jquery.com/) validation scripts. When the `Create` and `Edit` pages are added later in the tutorial, the *_ValidationScriptsPartial.cshtml* file will be used.

`Index`, `Error`, and `Privacy` pages are provided to:

* `Index`: Start an app.
* `Error`: Display error information.
* `Privacy`: Specify details about the site's privacy policy.

For this tutorial, the preceding pages are not used.

::: moniker-end
::: moniker range="= aspnetcore-2.1"

The default template creates **RazorPagesMovie**, **Home**, **About** and **Contact** links and pages. Depending on the size of your browser window, you might need to click the navigation icon to show the links.

![Home or Index page](../../tutorials/razor-pages/razor-pages-start/_static/home2.png)

Test the links. The **RazorPagesMovie** and **Home** links go to the Index page. The **About** and **Contact** links go to the `About` and `Contact` pages, respectively.

## Project files and folders

The following table lists the files and folders in the project. For this tutorial, the *Startup.cs* file is the most important to understand. You don't need to review each link provided below. The links are provided as a reference when you need more information on a file or folder in the project.

| File or folder              | Purpose |
| ----------------- | ------------ |
| wwwroot | Contains static files. See [Static files](xref:fundamentals/static-files). |
| Pages | Folder for [Razor Pages](xref:razor-pages/index). |
| *appsettings.json* | [Configuration](xref:fundamentals/configuration/index) |
| *Program.cs* | [Hosts](xref:fundamentals/host/index) the ASP.NET Core app.|
| *Startup.cs* | Configures services and the request pipeline. See [Startup](xref:fundamentals/startup).|

### The Pages folder

The *_Layout.cshtml* file contains common HTML elements (scripts and stylesheets) and sets the layout for the application. For example, when you click on **RazorPagesMovie**, **Home**, **About** or **Contact**, you see the same elements. The common elements include the navigation menu on the top and the header on the bottom of the window. See [Layout](xref:mvc/views/layout) for more information.

The *_ViewImports.cshtml* file contains Razor directives that are imported into each Razor Page. See [Importing Shared Directives](xref:mvc/views/layout#importing-shared-directives) for more information.

The *_ViewStart.cshtml* sets the Razor Pages `Layout` property to use the *_Layout.cshtml* file. See [Layout](xref:mvc/views/layout) for more information.

The *_ValidationScriptsPartial.cshtml* file provides a reference to [jQuery](https://jquery.com/) validation scripts. When we add `Create` and `Edit` pages later in the tutorial, the *_ValidationScriptsPartial.cshtml* file will be used.

The `About`, `Contact` and `Index` pages are basic pages you can use to start an app. The `Error` page is used to display error information. The `Privacy` page allows you to specify details about your site's privacy policy.

::: moniker-end

# [Visual Studio](#tab/visual-studio)

<a name="f7"></a>
## Use F7 to toggle between a Razor Page and the PageModel

F7 toggles between a Razor Page (*\*.cshtml* file) and the C# file (*\*.cshtml.cs*).

# [Visual Studio Code](#tab/visual-studio-code)

<!-- TODO review  Need something in these tabs -->

By convention, the Razor Page (*\*.cshtml* file) and the associated `PageModel` have the same root file name.

# [Visual Studio for Mac](#tab/visual-studio-mac)

By convention, the Razor Page (*\*.cshtml* file) and the associated `PageModel` have the same root file name.

---

> [!div class="step-by-step"]
> [Next: Adding a model](xref:tutorials/razor-pages/model)