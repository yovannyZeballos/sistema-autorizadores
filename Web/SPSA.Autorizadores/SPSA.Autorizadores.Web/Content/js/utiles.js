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

const quitarTildes = function (texto) {
    return texto.replace("á", "a").replace("é", "e").replace("í", "i").replace("ó", "o").replace("ú", "u");
}