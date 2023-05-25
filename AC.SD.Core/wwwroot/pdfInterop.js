window.displayPdf = function (base64Data) {
    // Decode the base64 data
    var pdfData = atob(base64Data);

    // Load the PDF data into PDF.js
    pdfjsLib.getDocument({ data: pdfData }).promise.then(function (pdf) {
        // Get the PDF document's first page
        pdf.getPage(1).then(function (page) {
            var scale = 1.5;
            var viewport = page.getViewport({ scale: scale });

            // Create a canvas element to render the page
            var canvas = document.createElement("canvas");
            var context = canvas.getContext("2d");
            canvas.height = viewport.height;
            canvas.width = viewport.width;

            // Render the PDF page onto the canvas
            var renderContext = {
                canvasContext: context,
                viewport: viewport
            };
            page.render(renderContext).promise.then(function () {
                // Append the canvas to the DOM
                var pdfContainer = document.getElementById("pdfContainer");
                pdfContainer.appendChild(canvas);
            });
        });
    });
};




