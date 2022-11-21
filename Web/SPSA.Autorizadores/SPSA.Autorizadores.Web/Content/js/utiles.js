const btnLoading = function (button, state) {
    if (state === true) {
        button.addClass("btn-loading");
    }
    else {
        button.removeClass("btn-loading");
    }
}