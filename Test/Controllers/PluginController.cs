using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DynamicPlugin;
using Microsoft.AspNetCore.Mvc;
using Plugin;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PluginController : ControllerBase
    {
        private readonly IPluginLoader _pluginLoader;

        public PluginController(IPluginLoader pluginLoader)
        {
            _pluginLoader = pluginLoader;
        }

        [HttpGet]
        public List<string> Get()
        {
            return _pluginLoader.GetLoadedPlugins().ToList();
        }

        [HttpGet("Load")]
        public IActionResult Load(bool forceReload = false)
        {
            var pluginName = "DemoPlugin001";
            var pluginNamespace = "DemoPlugin.DemoPlugin";
            var parent = Directory.GetParent(Environment.CurrentDirectory)?.FullName;
            var filePath = Path.Join(new[] { parent, "/DemoPlugin/bin/Debug/netstandard2.0/DemoPlugin.dll" });
            var plugin = _pluginLoader.LoadPlugin<IPlugin>(pluginName, filePath, pluginNamespace,
                DateTimeOffset.Now.AddSeconds(10), forceReload);
            return Ok(plugin.Hello("test"));
        }

        [HttpGet("LoadFromMemory")]
        public IActionResult LoadFromMemory()
        {
            var pluginName = "DemoPlugin001";
            var plugin = _pluginLoader.LoadPlugin<IPlugin>(pluginName);
            return Ok(plugin?.Hello("test"));
        }

        [HttpGet("remove")]
        public IActionResult Remove()
        {
            _pluginLoader.RemovePlugin("DemoPlugin001");
            return Ok();
        }

        [HttpGet("removes")]
        public IActionResult Removes()
        {
            _pluginLoader.RemovePlugins(new List<string>() { "DemoPlugin001" });
            return Ok();
        }
        
        [HttpGet("clean")]
        public IActionResult CLean()
        {
            _pluginLoader.Clean();
            return Ok();
        }
    }
}