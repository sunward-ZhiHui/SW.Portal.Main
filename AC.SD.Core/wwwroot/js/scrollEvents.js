
//window.scrollToBottom = {
//    initialize: function (element) {
//        element.addEventListener('scroll', function () {
//            if (element.scrollHeight - element.scrollTop === element.clientHeight) {
//                element.click();
//            }
//        });
//    },

//    isScrolledToBottom: function (element) {
//        return element.scrollHeight - element.scrollTop === element.clientHeight;
//    }
//};

//window.setHeight = function (selector, height) {
//    var element = document.querySelector(selector);
//    if (element) {
//        element.style.height = height + 'px';
//    }
//};


window.setHeight = function (selector, height) {
    var elements = document.querySelectorAll(selector);
    if (elements.length > 0) {
        elements.forEach(function (element) {
            element.style.setProperty('height', height + 'px', 'important');
        });
    }
};



window.getElementHeight = function (selector) {
    var element = document.querySelector(selector);
    if (element) {
        return element.offsetHeight;
    }
    return 0;
};
