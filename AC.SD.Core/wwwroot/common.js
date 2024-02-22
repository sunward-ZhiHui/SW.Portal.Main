function setGridHeight() {
    var height = window.innerHeight;
    var grid = document.getElementById('myGrid');
    var gridHeight = grid.getBoundingClientRect().top + height;
    grid.style.height = gridHeight + 'px';
}

function myFunction() {
    alert(1);
}
// hoverFunctions.js
window.hoverFunctions = {
    addHoverListener: function (element, dotNetObject) {
        element.addEventListener("mouseover", function () {
            dotNetObject.invokeMethodAsync('OnMouseOver');
        });
        element.addEventListener("mouseleave", function () {
            dotNetObject.invokeMethodAsync('OnMouseLeave');
        });
    }
};

function scrollToBottom() {
    window.scrollTo(0, document.body.msgOpen);
}

function setRichEditHeight(elementId) {
    const richEditElement = document.getElementById(elementId);
    if (richEditElement) {
        richEditElement.style.height = "100%";
    }
}
function printPage() {
    window.print();
}
function printFullPage() {
    // Use the 'print.js' library here
    printJS({
        printable: 'tableData', // ID of the content to print
        type: 'html',
    });
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


window.toastInterop = {
    initialize: function (containerId) {
        const container = document.getElementById(containerId);
        container.addEventListener('animationend', function (event) {
            if (event.animationName === 'fadeOut') {
                container.removeChild(event.target);
            }
        });
    },
    showToast: function (message, severity) {
        const toast = document.createElement('div');
        toast.className = `toast toast-${severity}`;
        toast.textContent = message;

        const progress = document.createElement('div');
        progress.className = 'toast-progress';
        toast.appendChild(progress);

        const container = document.querySelector('.toast-container');
        container.appendChild(toast);

        setTimeout(function () {
            toast.classList.add('fadeOut');
        }, 5000);
    }
};


//let slides = document.getElementsByClassName("slider__slide");
//let navlinks = document.getElementsByClassName("slider__navlink");
//let currentSlide = 0;
//function eventHandler(event)
//{
//    document.getElementById("nav-button--next").addEventListener("click", () => {
//        changeSlide(currentSlide + 1)
//    });
//    document.getElementById("nav-button--prev").addEventListener("click", () => {
//        changeSlide(currentSlide - 1)
//    });

//}

//function changeSlide(moveTo) {
//    if (moveTo >= slides.length) { moveTo = 0; }
//    if (moveTo < 0) { moveTo = slides.length - 1; }

//    slides[currentSlide].classList.toggle("active");
//    navlinks[currentSlide].classList.toggle("active");
//    slides[moveTo].classList.toggle("active");
//    navlinks[moveTo].classList.toggle("active");

//    currentSlide = moveTo;
////}

//document.querySelectorAll('.slider__navlink').forEach((bullet, bulletIndex) => {
//    bullet.addEventListener('click', () => {
//        if (currentSlide !== bulletIndex) {
//            changeSlide(bulletIndex);
//        }
//    })
//})
let slideIndex = 1;
showSlides(slideIndex);

function plusSlides(n) {
    showSlides(slideIndex += n);
}

function currentSlide(n) {
    showSlides(slideIndex = n);
}

function showSlides(n) {
    //let i;
    //let slides = document.getElementsByClassName("mySlides");
    //let dots = document.getElementsByClassName("dot");
    //if (n > slides.length) { slideIndex = 1 }
    //if (n < 1) { slideIndex = slides.length }
    //for (i = 0; i < slides.length; i++) {
    //    slides[i].style.display = "none";
    //}
    //for (i = 0; i < dots.length; i++) {
    //    dots[i].className = dots[i].className.replace(" active", "");
    //}
    //slides[slideIndex - 1].style.display = "block";
    //dots[slideIndex - 1].className += " active";
}

function ReloadUrl() {

    location.reload();
}
/*function elementBySrcId () {
        var idValue = document.getElementById("onCopyImagesOn");
        return idValue.src;
}*/
/*window.elementBySrcId = function () {
    var idValue = document.getElementById("onCopyImagesOn");
    console.log(idValue);
    return idValue.src;
}*/
async function interopDuringOnInit() {
    var srcs = document.getElementById("onCopyImagesOn").src;
    var res = await getBase64FromUrl(srcs);
    console.log(res);
    return res;
}
function string2Bin(str) {
    var result = [];
    for (var i = 0; i < str.length; i++) {
        result.push(str.charCodeAt(i).toString(2));
    }
    return result;
}
async function getClipboardImage() {
    const items = await navigator.clipboard.read();
    for (const item of items) {
        const blob = await item.getType('image/png');
        const reader = new FileReader();
        reader.readAsDataURL(blob);
        return new Promise((resolve, reject) => {
            reader.onloadend = () => {
                console.log(reader.result);
                document.getElementById("onCopyImagesOn").src = reader.result;
                resolve(reader.result);
            };
        });
    }
}
function retrieveImageFromClipboardAsBlob(pasteEvent, callback) {
    if (pasteEvent.clipboardData == false) {
        if (typeof (callback) == "function") {
            callback(undefined);
        }
    };

    var items = pasteEvent.clipboardData.items;

    if (items == undefined) {
        if (typeof (callback) == "function") {
            callback(undefined);
        }
    };

    for (var i = 0; i < items.length; i++) {
        // Skip content if not image
        if (items[i].type.indexOf("image") == -1) continue;
        // Retrieve image on clipboard as blob
        var blob = items[i].getAsFile();

        if (typeof (callback) == "function") {
            callback(blob);
        }
    }
}
const getBase64FromUrl = async (url) => {
    const data = await fetch(url);
    const blob = await data.blob();
    return new Promise((resolve) => {
        const reader = new FileReader();
        reader.readAsDataURL(blob);
        reader.onloadend = () => {
            const base64data = reader.result;
            resolve(base64data)
        };
    });
};
window.addEventListener("paste", function (e) {

    // Handle the event
    retrieveImageFromClipboardAsBlob(e, function (imageBlob) {
        // If there's an image, display it in the canvas
        if (imageBlob) {
            var canvas = document.getElementById("mycanvasss");
            var ctx = canvas.getContext('2d');

            // Create an image to render the blob on the canvas
            var img = new Image();

            // Once the image loads, render the img on the canvas
            img.onload = function () {
                // Update dimensions of the canvas with the dimensions of the image
                canvas.width = this.width;
                canvas.height = this.height;

                // Draw the image
                ctx.drawImage(img, 0, 0);
            };

            // Crossbrowser support for URL
            var URLObj = window.URL || window.webkitURL;

            // Creates a DOMString containing a URL representing the object given in the parameter
            // namely the original Blob
            img.src = URLObj.createObjectURL(imageBlob);
            document.getElementById("onCopyImagesOn").src = img.src;
            console.log(img.src);
        }
    });
}, false);




