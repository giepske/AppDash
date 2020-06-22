var sizes = ["B", "KiB", "MiB", "GiB", "TiB"];

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

window.torrentPluginSetChartCallback = async (chartId) => {
    while (window[ChartJsInterop.name].BlazorCharts.size < 1) {
        await sleep(10);
    }

    var chart = window[ChartJsInterop.name].BlazorCharts.get(chartId);

    chart.options.scales.yAxes[0].ticks.callback = function(value, index, values) {
        var len = value;
        var order = 0;
        while (len >= 1024 && order < sizes.length - 1) {
            order++;
            len = len / 1024;
        }

        return len.toFixed(2) + " " + sizes[order];
    }

    //todo see if this can removed without breaking anything
    chart.update();
}