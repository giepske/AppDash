using System.IO;
using AppDash.Core;

namespace AppDash.Server.Plugins.Services
{
    /// <summary>
    /// A file service for checking if files exist on the disk and for loading them from the disk.
    /// </summary>
    public class FileService
    {
        private readonly string _basePluginPath = Path.Combine(Config.ApplicationDirectory, "plugins");

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

        /// <summary>
        /// Returns if the plugin css file exists on the disk, the css file should be in the /css/ folder with the exact same name as the plugin.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly (plugin folder name).</param>
        /// <param name="cssFileName">Filename with extension without directory.</param>
        /// <returns></returns>
        public bool PluginCssFileExists(string assemblyName, string cssFileName)
        {
            return File.Exists(Path.Combine(_basePluginPath, assemblyName, "css", cssFileName));
        }

        /// <summary>
        /// Load a plugin css file from the disk.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly (plugin folder name).</param>
        /// <param name="cssFileName">Filename with extension without directory.</param>
        public byte[] LoadPluginCssFile(string assemblyName, string cssFileName)
        {
            return File.ReadAllBytes(
                Path.Combine(_basePluginPath, assemblyName, "css", cssFileName));
        }

        /// <summary>
        /// Returns if the plugin js file exists on the disk, the js file should be in the /js/ folder with the exact same name as the plugin.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly (plugin folder name).</param>
        /// <param name="jsFileName">Filename with extension without directory.</param>
        /// <returns></returns>
        public bool PluginJsFileExists(string assemblyName, string jsFileName)
        {
            return File.Exists(Path.Combine(_basePluginPath, assemblyName, "js", jsFileName));
        }

        /// <summary>
        /// Load a plugin js file from the disk.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly (plugin folder name).</param>
        /// <param name="jsFileName">Filename with extension without directory.</param>
        public byte[] LoadPluginJsFile(string assemblyName, string jsFileName)
        {
            return File.ReadAllBytes(
                Path.Combine(_basePluginPath, assemblyName, "js", jsFileName));
        }
    }
}
