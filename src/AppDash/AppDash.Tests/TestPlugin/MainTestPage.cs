using System;
using AppDash.Plugins.Pages;

namespace AppDash.Tests.TestPlugin
{
    public class MainTestPage : RealtimePluginPage<TestPlugin, MainTestPageComponent>
    {
        public MainTestPage(TestPlugin plugin) : base(plugin)
        { }

        public override void Dispose()
        { }
    }
}