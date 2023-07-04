function setGridHeight() {
    var height = window.innerHeight;
    var grid = document.getElementById('myGrid');
    var gridHeight = grid.getBoundingClientRect().top + height;
    grid.style.height = gridHeight + 'px';
}

function myFunction() {
    alert(1);
}
function scrollToBottom() {
    window.scrollTo(0, document.body.msgOpen);
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

	
