window.attachHtmlEditorPasteResize = function (elementId, maxWidth) {
    const editorElement = document.getElementById(elementId);
    if (!editorElement) return;

    editorElement.addEventListener('paste', function () {
        setTimeout(() => {
            let imgs = editorElement.querySelectorAll('img');
            imgs.forEach(img => {
                img.style.width = maxWidth + 'px';
                img.style.height = 'auto';
            });
        }, 10); // small delay so image gets inserted first
    });
};
