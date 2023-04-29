function setGridHeight() {
    var height = window.innerHeight;
    var grid = document.getElementById('myGrid');
    var gridHeight = grid.getBoundingClientRect().top + height;
    grid.style.height = gridHeight + 'px';
}
