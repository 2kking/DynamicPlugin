using System;
using System.Collections.Generic;
using System.Reflection;

namespace DynamicPlugin
{
    public interface IPluginLoader
    {
        T LoadPlugin<T>(string name, string filePath = "", string pluginNamespace = "",
            DateTimeOffset? absoluteExpiration = null,
            bool forceReload = false);

        T LoadPlugin<T>(string name, byte[] bytes, string pluginNamespace,
            DateTimeOffset? absoluteExpiration = null,
            bool forceReload = false);

        T LoadPlugin<T>(string name, Assembly assembly, string pluginNamespace,
            DateTimeOffset? absoluteExpiration = null,
            bool forceReload = false);

        HashSet<string> GetLoadedPlugins();

        void RemovePlugin(string name);

        void RemovePlugins(List<string> names);

        void Clean();
    }
}