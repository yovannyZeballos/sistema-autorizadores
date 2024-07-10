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

const mensajeError = function (mensaje) {
    swal({
        text: mensaje,
        icon: "error"
    });
}

const mensajeExito = function (mensaje) {
    swal({
        text: mensaje,
        icon: "success"
    });
}

const mensajeAdvertencia = function (mensaje) {
    swal({
        text: mensaje,
        icon: "warning"
    });
}

const convertirfecha = function (fecha) {

    if (fecha == null || fecha == "") return "";
    var day = fecha.getDate();       // yields date
    var month = fecha.getMonth() + 1;    // yields month (add one as '.getMonth()' is zero indexed)
    var year = fecha.getFullYear();  // yields year
    var hour = fecha.getHours();     // yields hours 
    var minute = fecha.getMinutes(); // yields minutes
    var second = fecha.getSeconds(); // yields seconds

    // After this construct a string with the above results as below
    return day + "/" + month + "/" + year + " " + hour + ':' + minute + ':' + second;
}