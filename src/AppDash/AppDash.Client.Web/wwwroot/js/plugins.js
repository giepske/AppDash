function loadPluginCssFile(cssFile) {
    var link = document.createElement('link');
    link.rel = 'stylesheet';
    link.href = cssFile;
    document.head.appendChild(link);
}

function loadPluginJsFile(jsFile) {
    var script = document.createElement('script');
    script.src = jsFile;
    document.head.appendChild(script);
}