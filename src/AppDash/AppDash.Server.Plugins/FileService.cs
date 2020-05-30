using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AppDash.Server.Plugins
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

        public FileMemoryService(FileService fileService)
        {
            _fileService = fileService;
            _pluginFiles = new Dictionary<string, byte[]>();
            _pluginIconFiles = new Dictionary<string, byte[]>();
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

        public byte[] GetPluginIcon(string pluginKey)
        {
            return _pluginIconFiles[pluginKey];
        }
    }

    /// <summary>
    /// A file service for checking if files exist on the disk and for loading them from the disk.
    /// </summary>
    public class FileService
    {
        private readonly string _basePluginPath = Path.Combine(AppContext.BaseDirectory, "plugins");

        /// <summary>
        /// Returns if the plugin file (.dll) exists on the disk, the folder name should be the same as the file name minus the extension.
        /// </summary>
        /// <param name="filename">Filename with extension without directory.</param>
        /// <returns></returns>
        public bool PluginFileExists(string filename)
        {
            return File.Exists(Path.Combine(_basePluginPath, Path.GetFileNameWithoutExtension(filename), filename));
        }

        /// <summary>
        /// Load a plugin file (.dll) from the disk.
        /// </summary>
        /// <param name="filename">Filename with extension without directory.</param>
        public byte[] LoadPluginFile(string filename)
        {
            return File.ReadAllBytes(
                Path.Combine(_basePluginPath, Path.GetFileNameWithoutExtension(filename), filename));
        }

        /// <summary>
        /// Returns if the plugin icon file exists on the disk, the icon should be in the /icons/ folder.
        /// </summary>
        /// <param name="pluginFileName">Plugin filename without extension.</param>
        /// <param name="iconFileName">Filename with extension without directory.</param>
        /// <returns></returns>
        public bool PluginIconFileExists(string pluginFileName, string iconFileName)
        {
            return File.Exists(Path.Combine(_basePluginPath, pluginFileName, "icons", iconFileName));
        }

        /// <summary>
        /// Load a plugin icon file from the disk.
        /// </summary>
        /// <param name="pluginFileName">Plugin filename with extension without directory.</param>
        /// <param name="iconFileName">Icon filename with extension without directory.</param>
        public byte[] LoadPluginIconFile(string pluginFileName, string iconFileName)
        {
            return File.ReadAllBytes(
                Path.Combine(_basePluginPath, pluginFileName, "icons", iconFileName));
        }
    }
}
