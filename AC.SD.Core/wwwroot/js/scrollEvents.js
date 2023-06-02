window.scrollToElement = function (selector) {
    var element = document.querySelector(selector);
    if (element) {
        var offset = element.offsetTop;
        window.scrollTo({
            top: offset,
            behavior: 'smooth'
        });
    }
};
