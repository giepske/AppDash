using System;
using AppDash.Plugins.Pages;

namespace AppDash.Tests.TestPlugin
{
    public class TestPage : RealtimePluginPage<TestPlugin, TestPageComponent>
    {
        public TestPage(TestPlugin plugin) : base(plugin)
        { }

        public override void Dispose()
        { }
    }
}