window.loadViewerJS = function () {
    var head = document.getElementsByTagName('head')[0];

    // Load ViewerJS CSS
    var link = document.createElement('link');
    link.rel = 'stylesheet';
    link.type = 'text/css';
    link.href = '/viewerjs/viewer.css';
    head.appendChild(link);

    // Load ViewerJS JavaScript
    var script = document.createElement('script');
    script.src = '/viewerjs/viewer.js';
    head.appendChild(script);
};

window.openViewer = function (containerId, documentUrl) {
    var viewerContainer = document.getElementById(containerId);
    var viewerOptions = {
        url: documentUrl
    };
    var viewer = new Viewer(viewerContainer, viewerOptions);
};
