var Home = function () {

    var eventos = function () {
        //Logout
        $("#btnCerrarSesion").on("click", function () {
            var urlLogout = baseUrl + 'Login/Logout';
            swal({
                title: "Confirmar!",
                text: "¿Está seguro de salir del sistema?",
                icon: "warning",
                buttons: ["No", "Si"],
                dangerMode: true,
            })
                .then((willDelete) => {
                    if (willDelete) {
                        window.location.href = urlLogout;
                    } else {

                    }
                });
        });
    }

    return {
        init: function () {
            checkSession(function () {
                eventos();


            });
        }
    };

}(jQuery);
$(function () { Home.init(); });