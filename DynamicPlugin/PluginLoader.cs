using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;

namespace DynamicPlugin
{
    public class PluginLoader : IPluginLoader
    {
        private IMemoryCache _cache;

        private HashSet<string> plugins = new HashSet<string>();

        public PluginLoader(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// load plugin with file path
        /// </summary>
        /// <param name="name">id</param>
        /// <param name="filePath">file path</param>
        /// <param name="pluginNamespace">plugin namespace</param>
        /// <param name="absoluteExpiration">expiration</param>
        /// <param name="forceReload">from memory or path</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadPlugin<T>(string name, string filePath = "", string pluginNamespace = "",
            DateTimeOffset? absoluteExpiration = null,
            bool forceReload = false)
        {
            var assembly = !string.IsNullOrEmpty(filePath) ? Assembly.LoadFile(filePath) : null;
            return LoadPlugin<T>(name, assembly, pluginNamespace, absoluteExpiration, forceReload);
        }

        /// <summary>
        /// load plugin with file byte array
        /// </summary>
        /// <param name="name">id</param>
        /// <param name="bytes">byte array</param>
        /// <param name="pluginNamespace">plugin namespace</param>
        /// <param name="absoluteExpiration">expiration</param>
        /// <param name="forceReload">from memory or path</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadPlugin<T>(string name, byte[] bytes = null, string pluginNamespace = "",
            DateTimeOffset? absoluteExpiration = null,
            bool forceReload = false)
        {
            var assembly = bytes != null ? Assembly.Load(bytes) : null;
            return LoadPlugin<T>(name, assembly, pluginNamespace, absoluteExpiration, forceReload);
        }

        /// <summary>
        /// load plugin with assembly
        /// </summary>
        /// <param name="name">id</param>
        /// <param name="assembly">assembly</param>
        /// <param name="pluginNamespace">plugin namespace</param>
        /// <param name="absoluteExpiration">expiration</param>
        /// <param name="forceReload">from memory or path</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadPlugin<T>(string name, Assembly assembly, string pluginNamespace,
            DateTimeOffset? absoluteExpiration = null,
            bool forceReload = false)
        {
            var cacheKey = $"plugin:{name}";
            if (!forceReload && _cache.TryGetValue(cacheKey, out T plugin))
            {
                return plugin;
            }

            if (assembly == null)
            {
                return default(T);
            }
            
            plugin = (T) assembly.CreateInstance(pluginNamespace);

            if (plugin != null)
            {
                if (absoluteExpiration != null)
                {
                    _cache.Set(cacheKey, plugin, (DateTimeOffset) absoluteExpiration);
                }
                else
                {
                    _cache.Set(cacheKey, plugin);
                }

                plugins.Add(name);
            }

            return plugin;
        }

        /// <summary>
        /// sync plugin with cache
        /// </summary>
        private void SyncPlugins()
        {
            plugins.RemoveWhere(e => !_cache.TryGetValue($"plugin:{e}", out _));
        }

        /// <summary>
        /// get loaded plugins
        /// </summary>
        /// <returns></returns>
        public HashSet<string> GetLoadedPlugins()
        {
            SyncPlugins();
            return plugins;
        }

        /// <summary>
        /// remove one plugin
        /// </summary>
        /// <param name="name">id</param>
        public void RemovePlugin(string name)
        {
            _cache.Remove($"plugin:{name}");
            plugins.Remove($"plugin:{name}");
        }

        /// <summary>
        ///  remove multiple plugins
        /// </summary>
        /// <param name="names"></param>
        public void RemovePlugins(List<string> names)
        {
            names?.ForEach(RemovePlugin);
        }

        /// <summary>
        /// remove all plugins
        /// </summary>
        public void Clean()
        {
            SyncPlugins();
            RemovePlugins(plugins.ToList());
        }
    }
}