---
title: Access HttpContext in ASP.NET Core
author: coderandhiker
description: HttpContext in ASP.NET Core. HttpContext is not thread-safe and can throw NullReferenceException.
monikerRange: '>= aspnetcore-2.1'
ms.author: riande
ms.custom: mvc
ms.date: 5/5/2020
no-loc: [Home, Privacy, Kestrel, appsettings.json, "ASP.NET Core Identity", cookie, Cookie, Blazor, "Blazor Server", "Blazor WebAssembly", "Identity", "Let's Encrypt", Razor, SignalR]
uid: fundamentals/httpcontext
---
# Access HttpContext in ASP.NET Core

ASP.NET Core apps access `HttpContext` through the <xref:Microsoft.AspNetCore.Http.IHttpContextAccessor> interface and its default implementation <xref:Microsoft.AspNetCore.Http.HttpContextAccessor>. It's only necessary to use `IHttpContextAccessor` when you need access to the `HttpContext` inside a service.

## Use HttpContext from Razor Pages

The Razor Pages <xref:Microsoft.AspNetCore.Mvc.RazorPages.PageModel> exposes the <xref:Microsoft.AspNetCore.Mvc.RazorPages.PageModel.HttpContext> property:

```csharp
public class AboutModel : PageModel
{
    public string Message { get; set; }

    public void OnGet()
    {
        Message = HttpContext.Request.PathBase;
    }
}
```

## Use HttpContext from a Razor view

Razor views expose the `HttpContext` directly via a [RazorPage.Context](xref:Microsoft.AspNetCore.Mvc.Razor.RazorPage.Context) property on the view. The following example retrieves the current username in an intranet app using Windows Authentication:

```cshtml
@{
    var username = Context.User.Identity.Name;
    
    ...
}
```

## Use HttpContext from a controller

Controllers expose the [ControllerBase.HttpContext](xref:Microsoft.AspNetCore.Mvc.ControllerBase.HttpContext) property:

```csharp
public class HomeController : Controller
{
    public IActionResult About()
    {
        var pathBase = HttpContext.Request.PathBase;

        ...

        return View();
    }
}
```

## Use HttpContext from middleware

When working with custom middleware components, `HttpContext` is passed into the `Invoke` or `InvokeAsync` method and can be accessed when the middleware is configured:

```csharp
public class MyCustomMiddleware
{
    public Task InvokeAsync(HttpContext context)
    {
        ...
    }
}
```

## Use HttpContext from custom components

For other framework and custom components that require access to `HttpContext`, the recommended approach is to register a dependency using the built-in [dependency injection](xref:fundamentals/dependency-injection) container. The dependency injection container supplies the `IHttpContextAccessor` to any classes that declare it as a dependency in their constructors:

::: moniker range=">= aspnetcore-3.0"

```csharp
public void ConfigureServices(IServiceCollection services)
{
     services.AddControllersWithViews();
     services.AddHttpContextAccessor();
     services.AddTransient<IUserRepository, UserRepository>();
}
```

::: moniker-end

::: moniker range="< aspnetcore-3.0"

```csharp
public void ConfigureServices(IServiceCollection services)
{
     services.AddMvc()
         .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
     services.AddHttpContextAccessor();
     services.AddTransient<IUserRepository, UserRepository>();
}
```

::: moniker-end

In the following example:

* `UserRepository` declares its dependency on `IHttpContextAccessor`.
* The dependency is supplied when dependency injection resolves the dependency chain and creates an instance of `UserRepository`.

```csharp
public class UserRepository : IUserRepository
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserRepository(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void LogCurrentUser()
    {
        var username = _httpContextAccessor.HttpContext.User.Identity.Name;
        service.LogAccessRequest(username);
    }
}
```

## HttpContext access from a background thread

`HttpContext` isn't thread-safe. Reading or writing properties of the `HttpContext` outside of processing a request can result in a <xref:System.NullReferenceException>.

> [!NOTE]
> If your app generates sporadic `NullReferenceException` errors, review parts of the code that start background processing or that continue processing after a request completes. Look for mistakes, such as defining a controller method as `async void`.

To safely perform background work with `HttpContext` data:

* Copy the required data during request processing.
* Pass the copied data to a background task.
* Do ***not*** reference `HttpContext` data in parallel tasks. Extract the data needed from the context before starting the parallel tasks.

To avoid unsafe code, never pass the `HttpContext` into a method that performs background work. Pass the required data instead. In the following example, `SendEmailCore` is called to start sending an email. The `correlationId` is passed to `SendEmailCore`, not the `HttpContext`. Code execution doesn't wait for `SendEmailCore` to complete:

```csharp
public class EmailController : Controller
{
    public IActionResult SendEmail(string email)
    {
        var correlationId = HttpContext.Request.Headers["x-correlation-id"].ToString();

        _ = SendEmailCore(correlationId);

        return View();
    }

    private async Task SendEmailCore(string correlationId)
    {
        ...
    }
}
```

## Blazor and shared state

[!INCLUDE[](~/blazor/includes/security/blazor-shared-state.md)]
