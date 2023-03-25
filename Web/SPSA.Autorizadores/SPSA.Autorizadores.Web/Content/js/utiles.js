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
    swal({
        text: "Procesando...",
        content: divLoader,
        buttons: false,
        closeOnClickOutside: false,
        closeOnEsc: false,
        className: 'swal-wide',
        width: 200
    });
}

const closeLoading = function () {
    if (swal)
        swal.close();
}