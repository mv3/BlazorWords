function showToast(message, delay) {
    document.getElementById("toastMessage").innerHTML = message;
    var toastElList = [].slice.call(document.querySelectorAll('.toast'))
    var toastList = toastElList.map(function (toastEl) {
        return new bootstrap.Toast(toastEl, { delay: (delay * 1000) })
    })
    toastList.forEach(toast => toast.show())
}

function getHelp() {
    var myModal = new bootstrap.Modal(document.getElementById('helpModal'), {})
    myModal.show();
}

function showModal(name) {
    var myModal = new bootstrap.Modal(document.getElementById(name), {})
    myModal.show();
}

function copyToClipboard(message) {
    navigator.clipboard.writeText(message).then(function () {
    });
}

function shareStats(message) {
    if (navigator.share) {
        navigator.share({
            text: message,
            url: 'mattveit.com'
        }).then(() => {
            return "A";
        })
            .catch(() => { return "B"; });
    } else {

        return "C";
    }
}

function bumpTile(guessNumber, letter) {
    let aniTile = $('#guess-' + guessNumber + '-letter-' + letter);
    aniTile
        .animate(
            { scale: 1.1 },
            {
                duration: 50,
                step: function (now, fx) {
                    fx.start = 1;
                    $(this).css({ transform: 'scale(' + now + ')' });
                }
            }
        )
        .animate(
            { scale: 1 },
            {
                duration: 50,
                step: function (now, fx) {

                    fx.start = 1.1;
                    $(this).css({ transform: 'scale(' + now + ')' });
                }
            }
        );
}

function flipTile(guessNumber, letter, color, border) {
    let aniTile = $('#guess-' + guessNumber + '-letter-' + letter);
    aniTile.delay(200 * letter)
        .animate(
            { deg: 90 },
            {
                duration: 300,
                step: function (now) {
                    $(this).css({ transform: 'rotateX(' + now + 'deg)' });
                }
            }
        )
        .queue(function (next) {
            $(this).css("background-color", color);
            $(this).css("border-color", border);
            next();
        })
        .animate(
            { deg: 0 },
            {
                duration: 300,
                step: function (now) {
                    $(this).css({ transform: 'rotateX(' + now + 'deg)' });
                }
            }
        );
}

function bounceTile(guessNumber, letter) {
    let aniTile = $('#guess-' + guessNumber + '-letter-' + letter);
    aniTile.delay(1000)
        .animate(
            { px: -20 },
            {
                duration: 200,
                step: function (now) {
                    $(this).css({ transform: 'translateY(' + now + 'px)' });
                }
            }
        )
        .animate(
            { px: 0 },
            {
                duration: 300,
                step: function (now) {
                    $(this).css({ transform: 'translateY(' + now + 'px)' });
                }
            }
        );
}