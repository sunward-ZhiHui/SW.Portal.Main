window.displayOfficeDocument = function (documentUrl) {
    var officeContainer = document.getElementById("officeContainer");

    if (!officeContainer) {
        console.error("Office container element not found.");
        return;
    }

    // Create an iframe element to display the Office document
    var iframe = document.createElement("iframe");
    iframe.src = documentUrl;
    iframe.style.width = "100%";
    iframe.style.height = "100%";

    // Append the iframe to the container
    officeContainer.appendChild(iframe);
};
