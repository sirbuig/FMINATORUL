document.addEventListener('DOMContentLoaded', function () {
    const againBtn = document.getElementById('playAgain');
    const exitBtn = document.getElementById('exit');

    againBtn.addEventListener('click', function () {
        window.location.href = againBtn.getAttribute('data-url');
    });

    exitBtn.addEventListener('click', function () {
        window.location.href = exitBtn.getAttribute('data-url');
    });
});