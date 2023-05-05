const divLoader = document.getElementById("loader");

const btnLoading = function (button, state) {
    if (state === true) {
        button.addClass("btn-loading");
    }
    else {
        button.removeClass("btn-loading");
    }
}


const showLoading = function () {
    $("#modalLoading").modal('show');
}

const closeLoading = function () {
    $("#modalLoading").modal('hide');
}