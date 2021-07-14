---
no-loc: [Home, Privacy, Kestrel, appsettings.json, "ASP.NET Core Identity", cookie, Cookie, Blazor, "Blazor Server", "Blazor WebAssembly", "Identity", "Let's Encrypt", Razor, SignalR]
---

<a name="d128"></a>

## System.IO.IOException: The configured user limit (128) on the number of inotify instances has been reached

Disabling `reloadOnChange` can significantly reduce the number of opened files:

[!code-csharp[](~/includes/docker-128/Program.cs?highlight=14&name=snippet)]

The preceding code:

* Can help prevent the user limit (128) when the app doesn't depend on reloading the *appsettings.json* file.
* Reads *appsettings.json* as the last configuration provider, therefore settings in *appsettings.json* override those set in the environment and the command line. For more information, see [Configuration](xref:fundamentals/configuration/index).

To disable reloading configuration files from the environment, set `DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false`

For alternative approaches or to leave feedback on this problem, see [this GitHub issue](https://github.com/dotnet/AspNetCore.Docs/issues/19814).
