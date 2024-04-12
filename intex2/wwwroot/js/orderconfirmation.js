document.addEventListener('DOMContentLoaded', function () {
    var slider = document.querySelector('.js-satisfaction-slider');
    var satisfactionDisplay = document.querySelector('.js-satisfaction');
    var legoHead = document.querySelector('.lego-head');

    slider.addEventListener('input', function () {
        var per = slider.value / slider.max;
        var deg = 180 - 180 * per;
        satisfactionDisplay.textContent = Math.round(per * 20);
        legoHead.style.background = `linear-gradient(to top, rgba(248,80,50,${1 - per}) 0%, var(--lego-yellow) ${100 - 100 * per}%)`;
        document.documentElement.style.setProperty('--smile-rotation', `${deg}deg`);
    });
});
