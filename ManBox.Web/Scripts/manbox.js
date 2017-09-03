/// <reference path="jquery-1.9.1.intellisense.js" />
/// <reference path="jquery-ui-1.10.1.custom.js" />
/// <reference path="knockout-2.2.0.debug.js" />
/// <reference path="knockout.mapping-latest.debug.js" />

$.ajaxSetup({ cache: false });

var ManBox = (function () {

    var manBox = {};

    manBox.HighLightCart = function (element) {
        $(element).parents(".catalog-product").effect("transfer", { to: $("#subscr-selection") }, 500,
                function () {
                    $("#subscr-selection").effect("highlight", { color: "#ffeeaa" }, 2000);
                }
        );
    };

    // meant to only load an iframe content if it's to be shown
    manBox.LoadIFrames = function (element) {
        if (!$(element).hasClass('catalog-product')) {
            element = $(element).parents('.catalog-product');
        }

        $(element).find('iframe').prop("src", function () {
            // Set their src attribute to the value of data-src
            return $(this).data("src");
        });
    };

    manBox.MakeCartStaticIfNeeded = function () {
        if ($("#subscr-selection").height() > $(window).height()) {
            $("#subscr-selection").css("position", "static");
        }
    };

    manBox.TrackEvent = function (event) {
        var action = $(event.toElement).data("track-action");
        analytics.track(action);
    };

    manBox.KonamiCode = function () {
        if (ko.dataFor($("#subscr-selection")[0]).SelectedProducts().length > 0) //if something in cart only
        {
            $.post("/box/applycoupon", { code: 'chucknorris3' }, function () {
                $('#chuckModal').modal({ remote: "/box/chuck" });

                // refresh model
                $.getJSON("/box/getsubscription", function (data) {
                    if (data) {
                        selectionModel = ko.mapping.fromJS(data, selectionMapping);
                    }
                });
            });
        }
        else {
            alert('add something in your cart first');
        }
    };

    manBox.ShowWelcomeMsg = function () {
        if (!$.cookie('nowelcome')) {
            $('#welcomeModal').modal();
        }
    };


    return manBox;
})();



// DOM ANIMATION EVENTS
$(document).ready(function () {

    $("body").find("[data-track-action]").on("click", function (event) { ManBox.TrackEvent(event); });

    // delivery info modal
    $(".delivery").click(
        function () {
            $('#deliveryModal').modal();
            ManBox.TrackEvent("deliveryInfoClick");
        }
    );

    $(".verif-code-info").click(
        function () {
            $('#verifCodeModal').modal();
            ManBox.TrackEvent("verifCodeInfoClick");
        }
    );

    // clear zoom modal content otherwise it's not reloaded on next zoom
    $('#zoomModal').on('hidden', function () {
        $(this).data('modal').$element.removeData();
    });

    // accept terms in checkout: clicking anywhere on the box ticks the checkbox
    var acceptVal = false;
    $('.accept-terms').click(function () {
        if (!acceptVal) {
            acceptVal = true;
        } else {
            acceptVal = false;
        }

        $(this).siblings('input[type="checkbox"]').prop('checked', acceptVal);
    });

    // go to cart button for phones
    $('.phone-goto-cart').click(function () {
        window.scrollTo(0, $('.cart').position().top - 50);
    });

    // show coupon in checkout
    $('.show-coupon').click(function () {
        $(this).hide();
        $('#coupon-form').show();
    });

    // order detail
    $('.show-order-detail').click(function () {
        var elem = $(this).parents('tr:first').next('tr.order-detail');
        elem.fadeToggle().removeClass('show-detail-override');
    });

    // snooze confirm
    $('.btn-rush').click(function () { $('.rush, .delivery-actions-options').fadeIn(); $('.snooze').hide(); });
    $('.btn-snooze').click(function () { $('.rush').hide(); $('.snooze, .delivery-actions-options').fadeIn(); });

    // thumbnails for zoom
    $('#zoomModal').on('click', '.thumb-picture', function () {
        $('.thumb-active').removeClass('thumb-active');
        $(this).addClass('thumb-active');
        $('.zoom-area iframe').remove();
        var zoomSrc = $(this).find('img').data('zoom-src');
        $('.zoom-area img').show().attr('src', zoomSrc);
    });

    // thumbnail for vine
    $('#zoomModal').on('click', '.thumb-video', function () {
        $('.thumb-active').removeClass('thumb-active');
        $(this).addClass('thumb-active');
        $('.zoom-area iframe').remove();
        $('.zoom-area img').hide();
        var vineSrc = $(this).find('img').data('vine-src');
        $('.zoom-area').append('<iframe class="vine-embed" src="' + vineSrc + '" width="600" height="600" frameborder="0"></iframe>');
    });

    $('#welcomeModal form').submit(function (event) {
        event.preventDefault();
        if ($(this).valid()) {
            $.post($(this).attr('action'), $(this).serialize(), function () {
                $.cookie('nowelcome', true);
                $('form, .welcome-newsletter h4').hide();
                $('.welcome-newsletter .alert').fadeIn();
                setTimeout(function () { $('#welcomeModal').modal('toggle'); }, 2500);
            });
        }
    });

    var konami = new Konami(ManBox.KonamiCode);

    $('#packs .product-image').popover({ content: $(this).children().first().html() });
    $('#packs .deal i').tooltip();

    $('.skip-packs').click(function () {
        $("body, html").animate({
            scrollTop: $("#compose").offset().top //nav main height offset
        }, 800);
        return false;
    });

    $('.show-packs').click(function () {
        $('#packs').slideDown('slow');
        $("body").animate({
            scrollTop: 0 //nav main height offset
        }, 800);

        $('*[data-spy=affix]')
            .attr("data-offset-top", "750").affix();

        return false;
    });

    $('.pack-col').hover(function () {
        $('.activePack').removeClass('activePack');
        $(this).addClass('activePack');
    });

    $('#faq a.faq-question').click('fast', function () {
        $(this).next('.faq-answer').fadeToggle();
        if ($(this).next('.faq-answer').is(":visible")) {
            $(this).find('.icon-plus-sign').addClass('icon-minus-sign').removeClass('icon-plus-sign');
        } else {
            $(this).find('.icon-minus-sign').addClass('icon-plus-sign').removeClass('icon-minus-sign');
        }
    });

    $('#faq a.show-all').click(function () { $('.faq-answer').fadeIn(); });
    $('#faq a.hide-all').click(function () { $('.faq-answer').fadeOut(); });
});



