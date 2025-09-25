window.resizeImageOnPaste = function (elementId, maxWidth, quality) {
    // RichEdit is inside an iframe — find its document
    const container = document.getElementById(elementId);
    if (!container) return;

    const iframe = container.querySelector("iframe");
    if (!iframe || !iframe.contentDocument) return;

    const editorDoc = iframe.contentDocument;

    editorDoc.addEventListener("paste", function (event) {
        if (event.clipboardData && event.clipboardData.items) {
            for (let item of event.clipboardData.items) {
                if (item.type.indexOf("image") === 0) {
                    event.preventDefault();
                    const blob = item.getAsFile();
                    const reader = new FileReader();

                    reader.onload = function (e) {
                        const img = new Image();
                        img.onload = function () {
                            let targetWidth = img.width;
                            let targetHeight = img.height;

                            // Resize if larger than maxWidth
                            if (img.width > maxWidth) {
                                const scale = maxWidth / img.width;
                                targetWidth = maxWidth;
                                targetHeight = img.height * scale;
                            }

                            const canvas = document.createElement("canvas");
                            canvas.width = targetWidth;
                            canvas.height = targetHeight;

                            const ctx = canvas.getContext("2d");
                            ctx.drawImage(img, 0, 0, targetWidth, targetHeight);

                            // Compress to JPEG
                            canvas.toBlob(function (compressedBlob) {
                                const newReader = new FileReader();
                                newReader.onloadend = function () {
                                    editorDoc.execCommand("insertImage", false, newReader.result);
                                };
                                newReader.readAsDataURL(compressedBlob);
                            }, "image/jpeg", quality);
                        };
                        img.src = e.target.result;
                    };
                    reader.readAsDataURL(blob);
                }
            }
        }
    });
};
