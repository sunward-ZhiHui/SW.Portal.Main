
window.scrollToBottom = {
    initialize: function (element) {
        element.addEventListener('scroll', function () {
            if (element.scrollHeight - element.scrollTop === element.clientHeight) {
                element.click();
            }
        });
    },

    isScrolledToBottom: function (element) {
        return element.scrollHeight - element.scrollTop === element.clientHeight;
    }
};
