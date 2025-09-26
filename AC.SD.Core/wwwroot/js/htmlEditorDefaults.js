//window.attachPasteHandler = function (element, dotNetHelper) {
//    if (!element) return;
//    element._pasteHandler = function () {
//        dotNetHelper.invokeMethodAsync("OnEditorPaste");
//    };
//    element.addEventListener("paste", element._pasteHandler);
//};

//window.detachPasteHandler = function (element) {
//    if (element && element._pasteHandler) {
//        element.removeEventListener("paste", element._pasteHandler);
//        delete element._pasteHandler;
//    }
//};


// Wait for all images inside the editor to decode, then return content height
window.measureEditorHeightAfterImages = async function (container, extraPadding = 80) {
    if (!container) return 500;

    // Find the editable content area (works with DevExpress HtmlEditor)
    const content = container.querySelector('[contenteditable="true"]')
        || container.querySelector('.dxbl-html-editor'); // fallback

    if (!content) return 500;

    // Wait for images to finish decoding so scrollHeight is accurate
    const imgs = Array.from(content.querySelectorAll('img'));
    await Promise.all(imgs.map(img => (img.decode ? img.decode().catch(() => { }) :
        new Promise(res => img.complete ? res() : img.addEventListener('load', res, { once: true })))));

    // A tiny rAF helps after large DOM mutations
    await new Promise(r => requestAnimationFrame(r));

    const h = content.scrollHeight + Number(extraPadding || 0);
    return h > 300 ? h : 300; // keep a sensible minimum
};

// Optional: attach/detach paste handler that calls back into .NET
window.attachPasteHandler = function (element, dotNetHelper) {
    if (!element) return;
    element._pasteHandler = function () {
        // Just a signal; Blazor will call measureEditorHeightAfterImages afterwards
        dotNetHelper.invokeMethodAsync('OnEditorPaste');
    };
    element.addEventListener('paste', element._pasteHandler);
};

window.detachPasteHandler = function (element) {
    if (element && element._pasteHandler) {
        element.removeEventListener('paste', element._pasteHandler);
        delete element._pasteHandler;
    }
};

