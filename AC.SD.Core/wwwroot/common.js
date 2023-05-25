function setGridHeight() {
    var height = window.innerHeight;
    var grid = document.getElementById('myGrid');
    var gridHeight = grid.getBoundingClientRect().top + height;
    grid.style.height = gridHeight + 'px';
}

function myFunction() {
    alert(1);
}

function setRichEditHeight(elementId) {
    const richEditElement = document.getElementById(elementId);
    if (richEditElement) {
        richEditElement.style.height = "100%";
    }
}

window.downloadFile = (fileName, fileContent, contentType, folderPath) => {
    const blob = new Blob([fileContent], { type: contentType });
    const url = URL.createObjectURL(blob);

    const a = document.createElement("a");
    a.href = url;
    a.download = folderPath ? folderPath + "/" + fileName : fileName; // Append the folder path to the file name if provided
    a.style.display = "none";

    document.body.appendChild(a);
    a.click();

    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};


