using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ChartJs.Blazor.Charts;
using Microsoft.JSInterop;

namespace TorrentPlugin
{
    public static class ChartExtensions
    {
        public static async Task SetTickCallback(this ChartJsLineChart chart, string canvasId)
        {
            var type = chart.GetType();
            
            var fieldInfo = type.BaseType.GetProperty("JsRuntime", BindingFlags.NonPublic | BindingFlags.Instance);

            var jSRuntime = (IJSRuntime)fieldInfo.GetValue(chart);

            await jSRuntime.InvokeVoidAsync("torrentPluginSetChartCallback", canvasId);
        }
    }
}
