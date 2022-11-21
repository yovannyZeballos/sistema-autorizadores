function checkSession(callback) {
    $.ajax({
        url: baseUrl + "Login/VericarSession",
        type: "post",
        dataType: "json",
        success: function (response) {
            if (response.Ok == false) {
                redireccionarLogin();
            } else {
                if (callback != null && typeof (callback) == "function")
                    callback();
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });


 
}

function redireccionarLogin() {
    setTimeout(function () {
        var urlLogin = baseUrl + 'Login/Index';
        window.location.href = urlLogin;
    }, 100);
}