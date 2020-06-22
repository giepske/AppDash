using System.Collections.Generic;

namespace AppDash.Server.Plugins.Services
{
    /// <summary>
    /// A service for storing files into memory and reading from memory for fast transfer without hurting the disk.
    /// </summary>
    public class FileMemoryService
    {
        private readonly FileService _fileService;

        /// <summary>
        /// List of plugin files in bytes and their filename with extension as key.
        /// </summary>
        private readonly Dictionary<string, byte[]> _pluginFiles;

        /// <summary>
        /// List of plugin icon files in bytes and their plugin key.
        /// </summary>
        private readonly Dictionary<string, byte[]> _pluginIconFiles;

        /// <summary>
        /// List of plugin css files in bytes and their plugin name.
        /// </summary>
        private readonly Dictionary<string, byte[]> _pluginCssFiles;

        /// <summary>
        /// List of plugin js files in bytes and their plugin name.
        /// </summary>
        private readonly Dictionary<string, byte[]> _pluginJsFiles;

        public FileMemoryService(FileService fileService)
        {
            _fileService = fileService;
            _pluginFiles = new Dictionary<string, byte[]>();
            _pluginIconFiles = new Dictionary<string, byte[]>();
            _pluginCssFiles = new Dictionary<string, byte[]>();
            _pluginJsFiles = new Dictionary<string, byte[]>();
        }

        /// <summary>
        /// Check if a plugin file exists in memory, if not it will load it if it exists on the disk.
        /// </summary>
        /// <param name="filename">Filename with extension without directory.</param>
        /// <returns>Returns true if the file is loaded into memory.</returns>
        public bool CheckPluginFile(string filename)
        {
            if (_pluginFiles.ContainsKey(filename))
                return true;

            lock (_pluginFiles)
            {
                if (_fileService.PluginFileExists(filename))
                {
                    _pluginFiles[filename] = _fileService.LoadPluginFile(filename);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Get a plugin file from memory, assuming it exists in memory. Use <see cref="CheckPluginFile"/> to make sure it exists before using this method.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public byte[] GetPluginFile(string fileName)
        {
            return _pluginFiles[fileName];
        }

        /// <summary>
        /// Check if a plugin icon file exists in memory, if not it will load it if it exists on the disk.
        /// </summary>
        /// <param name="pluginKey">The plugin key.</param>
        /// <param name="pluginFileName">The plugin filename without extension.</param>
        /// <param name="iconFileName">The filename of the icon.</param>
        /// <returns>Returns true if the icon file is loaded into memory.</returns>
        public bool CheckIconFile(string pluginKey, string pluginFileName, string iconFileName)
        {
            if (_pluginIconFiles.ContainsKey(pluginKey))
                return true;

            lock (_pluginIconFiles)
            {
                if (_fileService.PluginIconFileExists(pluginFileName, iconFileName))
                {
                    _pluginIconFiles[pluginKey] = _fileService.LoadPluginIconFile(pluginFileName, iconFileName);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Get the icon file from memory, assuming it exists in memory. Use <see cref="CheckIconFile"/> to make sure it exists before using this method.
        /// </summary>
        /// <param name="pluginKey"></param>
        /// <returns></returns>
        public byte[] GetIconFile(string pluginKey)
        {
            return _pluginIconFiles[pluginKey];
        }

        /// <summary>
        /// Check if a plugin css file exists in memory, if not it will load it if it exists on the disk.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly (plugin folder name).</param>
        /// <param name="pluginName">The plugin name.</param>
        /// <param name="cssFileName">The filename of the css file.</param>
        /// <returns>Returns true if the icon file is loaded into memory.</returns>
        public bool CheckCssFile(string assemblyName, string pluginName, string cssFileName)
        {
            if (_pluginCssFiles.ContainsKey(pluginName))
                return true;

            lock (_pluginCssFiles)
            {
                if (_fileService.PluginCssFileExists(assemblyName, cssFileName))
                {
                    _pluginCssFiles[pluginName] = _fileService.LoadPluginCssFile(assemblyName, cssFileName);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Get the css file from memory, assuming it exists in memory. Use <see cref="CheckCssFile"/> to make sure it exists before using this method.
        /// </summary>
        /// <param name="pluginName"></param>
        /// <returns></returns>
        public byte[] GetCssFile(string pluginName)
        {
            return _pluginCssFiles[pluginName];
        }

        /// <summary>
        /// Check if a plugin css file exists in memory, if not it will load it if it exists on the disk.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly (plugin folder name).</param>
        /// <param name="pluginName">The plugin name.</param>
        /// <param name="jsFileName">The filename of the js file.</param>
        /// <returns>Returns true if the icon file is loaded into memory.</returns>
        public bool CheckJsFile(string assemblyName, string pluginName, string jsFileName)
        {
            if (_pluginJsFiles.ContainsKey(pluginName))
                return true;

            lock (_pluginJsFiles)
            {
                if (_fileService.PluginJsFileExists(assemblyName, jsFileName))
                {
                    _pluginJsFiles[pluginName] = _fileService.LoadPluginJsFile(assemblyName, jsFileName);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Get the js file from memory, assuming it exists in memory. Use <see cref="CheckJsFile"/> to make sure it exists before using this method.
        /// </summary>
        /// <param name="pluginName"></param>
        /// <returns></returns>
        public byte[] GetJsFile(string pluginName)
        {
            return _pluginJsFiles[pluginName];
        }
    }
}