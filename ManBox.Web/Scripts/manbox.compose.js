/// <reference path="jquery-1.9.1.js" />
/// <reference path="knockout-2.2.1.js" />
/// <reference path="knockout.mapping-latest.debug.js" />
/// <reference path="sammy-0.7.4.js" />

var selectionModel;

var extendedProduct = function (data) {
    ko.mapping.fromJS(data, {}, this);

    var that = this;

    this.quantity = ko.observableArray([1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);
    this.selectedQuantity = ko.observable();
    this.selectedModelId = ko.observable();
    this.selectModelSize = function (modelData, event) {
        that.selectedModelId(modelData.ModelId());
    };
};

var overviewMapping = {
    Products: {
        create: function (options) {
            return new extendedProduct(options.data);
        }
    }
};

// overviewData is defined in the compose view

var catalogOverviewModel = ko.mapping.fromJS(overviewData, overviewMapping);
catalogOverviewModel.activeProductDetail = ko.observable();

catalogOverviewModel.addModel = function (data, event) {

    var selectedModelId = data.selectedModelId();
    // one size 
    if(data.Models().length === 1)
    {
        selectedModelId = data.Models()[0].ModelId();
    }

    if (!selectedModelId) {
        selectedModelId = data.Models()[0].ModelId();
    }
    
        var originalEvent = event;
        $(event.currentTarget).button('loading'); // set button state as loading

        $.post("/box/addtosubscription", { modelId: selectedModelId, quantity: 1 }, function (result) {
            if (ko.dataFor($("#subscr-selection")[0])) {  // don't bind twice
                ko.mapping.fromJS(result, selectionMapping, selectionModel);
            } else {
                selectionModel = ko.mapping.fromJS(result, selectionMapping);
                ko.applyBindings(selectionModel, $("#subscr-selection")[0]);
            }

            $(event.currentTarget).button('reset'); // set button state back to normal once ajax call is done
            ManBox.HighLightCart(event.currentTarget);
            ManBox.MakeCartStaticIfNeeded();
        });
    
};

catalogOverviewModel.openProductDetail = function (data, event) {
    catalogOverviewModel.activeProductDetail(data.Reference());
    topPos = $(event.toElement).parents('.catalog-product').position().top;

    $("body").animate({
            scrollTop: topPos - 50 //nav main height offset
        }, 800);
    ManBox.LoadIFrames($(event.toElement));
    return false;
};

catalogOverviewModel.closeProductDetail = function (data, event) {
    catalogOverviewModel.activeProductDetail('');
    return false;
};

// TODO: show size aid by categoryId
catalogOverviewModel.showSizeAid = function (data, event) {
    $('#sizeChartModal').modal();
    return false;
};

catalogOverviewModel.activeCategory = ko.observable();
catalogOverviewModel.selectCategory = function (data) { window.location = data.TitleStd(); };
catalogOverviewModel.categoriesToShow = ko.computed(function () {
    var desiredCategory = catalogOverviewModel.activeCategory();
    var result = ko.utils.arrayFilter(catalogOverviewModel.ProductsCategories(), function (cat) {
        if (desiredCategory === 'all') {
            return true;
        }
        return cat.TitleStd() == desiredCategory;
    });

    if (!result) {
        result = catalogOverviewModel.ProductsCategories()[0]; // default category if wrong filter
    }

    return result;
});

ko.applyBindings(catalogOverviewModel, $("#catalog-overview")[0]);
ko.applyBindings(catalogOverviewModel, $("#categories-navigation")[0]);
ko.applyBindings(catalogOverviewModel, $("#packs")[0]);


// Subscription Model

var extSelectionModel = function (data) {
    ko.mapping.fromJS(data, {}, this);

    this.removeModel = function () {
        $.post("/box/removefromsubscription", { modelId: data.ModelId }, function (result) {
            ko.mapping.fromJS(result, selectionMapping, selectionModel);
        });
    };
}

var selectionMapping = {
    'SelectedProducts': {
        create: function (options) {
            return new extSelectionModel(options.data);
        }
    }
};

$.getJSON("/box/getsubscription", function (data) {
    console.log("get sub");
    if (data) {
        console.log("received sub");
        selectionModel = ko.mapping.fromJS(data, selectionMapping);

        // no cart
        if (selectionModel.SelectedProducts().length == 0 && !selectionModel.Pack) {
            setTimeout(function () {
                if (selectionModel.SelectedProducts().length == 0 && !selectionModel.Pack) {
                    ManBox.ShowWelcomeMsg();
                }
            }, 300000);
        }

        selectionModel.removePack = function () {
            $.post("/box/removepackfromsubscription", { packId: data.Pack.PackId }, function (result) {
                ko.mapping.fromJS(result, selectionMapping, selectionModel);
            });
        };

        selectionModel.validateCheckout = function (cartData) {
            if (cartData.IsSubscribed() && cartData.DeliveryState() !== 'New') {
                return false;
            }

            var parsedItemsTotal = parseFloat(cartData.ItemsTotal().replace(',','.').replace('€', '').replace(' ', ''));

            if (cartData.Token && parsedItemsTotal > 0) {
                return true;
            }

            return false;
        };

        ko.applyBindings(selectionModel, $("#subscr-selection")[0]);
    }
});


// Routing 
var app = Sammy(function () {
    var lang = '';

    this.get(':lang/box/compose/:category', function () {

        // if language changed reload page to correct language
        if (lang !== '' && lang !== this.params.lang) {
            window.location = '/' + this.params.lang + '/box/compose/' + this.params.category;
        }
        catalogOverviewModel.activeCategory(this.params.category);
        lang = this.params.lang;
    });

    this.get(':lang/box/compose', function () {
        this.app.runRoute('get', this.params.lang + '/box/compose/all')
    });

    this.get(':lang/box/compose#welcome', function () {
        ManBox.ShowWelcomeMsg();
        this.app.runRoute('get', this.params.lang + '/box/compose/all')
    });

    this.get(':lang/box/compose#_=_', function () {
        this.app.runRoute('get', this.params.lang + '/box/compose/all')
    });

    //regex for legacy routes 
    this.get(':lang/box/compose/.*/.*', function () {
        this.app.runRoute('get', this.params.lang + '/box/compose/all')
    });
});

app.run();
