using System;
using System.IO;
using System.Reflection;
using AppDash.Core;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestFramework("AppDash.Tests.PluginPacker", "AppDash.Tests")]

namespace AppDash.Tests
{
    public class PluginPacker : XunitTestFramework
    {
        public PluginPacker(IMessageSink messageSink)
            : base(messageSink)
        {
            var testAssembly = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;

            var testPath = Path.GetDirectoryName(testAssembly);

            var pluginDirectory = Path.Combine(testPath, "plugins", Path.GetFileNameWithoutExtension(testAssembly));

            Directory.CreateDirectory(Path.Combine(pluginDirectory));

            //remove old dll file if it exists
            if(File.Exists(Path.Combine(pluginDirectory, Path.GetFileName(testAssembly))))
                File.Delete(Path.Combine(pluginDirectory, Path.GetFileName(testAssembly)));

            File.Copy(testAssembly, Path.Combine(pluginDirectory, Path.GetFileName(testAssembly)));

            Directory.CreateDirectory(Path.Combine(pluginDirectory, "icons"));

            //copy the plugin icon if it doesn't exist
            if (!File.Exists(Path.Combine(pluginDirectory, "icons", "icon.png")))
                File.Copy(Path.Combine(testPath, "TestPlugin", "icons", "icon.png"), Path.Combine(pluginDirectory, "icons", "icon.png"));
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
