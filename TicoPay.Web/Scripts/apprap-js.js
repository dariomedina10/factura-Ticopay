/*

- Thank you for purchasing Apprap - App Landing Page Template from hBeThemes. 
- Youc an edit the slideshow images below.

*/


"use strict";








// Slideshow background image JS
(function() {
    var imgArray = [
        '../images/ticopay-facturacion-electronica-factura-principal-placeholder_01.jpg', // Replace with the URL for your first slideshow image
        '../images/ticopay-facturacion-electronica-factura-principal-placeholder_02.jpg', // Replace with the URL for your first slideshow image
        '../images/ticopay-facturacion-electronica-factura-principal-placeholder_03.jpg'], // Replace with the URL for your fourth slideshow image
        curIndex = 0,
        imgDuration = 10000;

    function slideShow() {
        document.getElementById('slider').className += "fadeOut";
        setTimeout(function() {
            document.getElementById('slider').src = imgArray[curIndex];
            document.getElementById('slider').className = "";
        },500);
        curIndex++;
        if (curIndex === imgArray.length) { curIndex = 0; }
        setTimeout(slideShow, imgDuration);
    }
    slideShow();
})();









/* You do not need to edit below this line. */











// Smooth scroll
$(function() {
    $('a[href*="#"]:not([href="#"])').on('click', function() {
        if (location.pathname.replace(/^\//,'') == this.pathname.replace(/^\//,'') && location.hostname == this.hostname) {
            var target = $(this.hash);
            target = target.length ? target : $('[name=' + this.hash.slice(1) +']');
            if (target.length) {
                $('html, body').animate({
                scrollTop: target.offset().top
                }, 1000);
                return false;
            }
        }
    });
});



// Reviews
(function() {

    var quotes = $(".testimonial");
    var quoteIndex = -1;
    
    function showNextQuote() {
        ++quoteIndex;
        quotes.eq(quoteIndex % quotes.length)
            .fadeIn(1000)
            .delay(6000)
            .fadeOut(1000, showNextQuote);
    }
    
    showNextQuote();
})();



// Popup JS
function openPopup(elem) {
   $(elem).next().fadeIn(100).siblings(".popup").hide();
}


function closePopup() {
    $('.popup').fadeOut(100);
}