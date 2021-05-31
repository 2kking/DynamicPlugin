### DynamicPlugin
[![NuGet](https://img.shields.io/nuget/v/Shadow.DynamicPlugin.svg)](https://www.nuget.org/packages/Shadow.DynamicPlugin/)
![](https://img.shields.io/nuget/dt/Shadow.DynamicPlugin.svg)

dynamic control of dll plugin for .net

#### Example

------

see details in Test

##### DI

```c#
public void ConfigureServices(IServiceCollection services)
{
    //MemoryCache
    services.AddMemoryCache();
    //PluginLoader
    services.AddSingleton<IPluginLoader, PluginLoader>();
}
```

##### Use

```c#
[Route("api/[controller]")]
[ApiController]
public class PluginController : ControllerBase
{
    private readonly IPluginLoader _pluginLoader;

    public PluginController(IPluginLoader pluginLoader)
    {
        _pluginLoader = pluginLoader;
    }

    [HttpGet("Load")]
    public IActionResult Load()
    {
        var plugin = _pluginLoader.LoadPlugin<IPlugin>(pluginName, filePath, pluginNamespace);
        return Ok(plugin.Hello("test"));
    }
}
```

