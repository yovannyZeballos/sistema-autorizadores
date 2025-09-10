$(document).ready(function () {
    // Nivel 1
    $('.side-menu__item').on('click', function (e) {
        e.preventDefault();
        const $submenu = $(this).next('.slide-menu');

        $('.slide-menu').not($submenu).slideUp();
        $('.side-menu__item').not(this).find('.angle').removeClass('rotate');

        $submenu.stop(true, true).slideToggle();
        $(this).find('.angle').toggleClass('rotate');
    });

    // Nivel 2
    $('.slide-menu').on('click', '.sub-side-menu__link', function (e) {
        const $submenu = $(this).next('.sub-sub-slide-menu');
        if ($submenu.length > 0) {
            e.preventDefault();

            $('.sub-sub-slide-menu').not($submenu).slideUp();
            $('.sub-side-menu__link').not(this).find('.angle').removeClass('rotate');

            $submenu.stop(true, true).slideToggle();
            $(this).find('.angle').toggleClass('rotate');
        }
    });

    // Activar según URL
    const path = window.location.pathname.toLowerCase();
    $('.side-menu a').each(function () {
        if (this.pathname.toLowerCase() === path) {
            $(this).addClass('active');

            const $nivel3 = $(this).closest('.sub-sub-slide-menu');
            const $nivel2 = $(this).closest('.slide-menu');

            if ($nivel3.length > 0) {
                $nivel3.show();
                $nivel3.closest('li.sub-slide').children('.sub-side-menu__link').find('.angle').addClass('rotate');
                $nivel3.closest('.slide-menu').show();
                $nivel3.closest('.slide').children('.side-menu__item').find('.angle').addClass('rotate');
            } else if ($(this).closest('li.sub-slide').length > 0) {
                $(this).closest('.slide-menu').show();
                $(this).closest('.slide').children('.side-menu__item').find('.angle').addClass('rotate');
            } else {
                $(this).closest('li.slide').children('.slide-menu').show();
                $(this).closest('li.slide').children('.side-menu__item').find('.angle').addClass('rotate');
            }
        }
    });
});
