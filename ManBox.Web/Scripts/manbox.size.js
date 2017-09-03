/// <reference path="jquery-1.9.1.js" />
/// <reference path="knockout-2.2.1.js" />
/// <reference path="knockout.mapping-latest.debug.js" />
/// <reference path="sammy-0.7.4.js" />

// Subscription Cart

var selectionModel;

var extSelectionModel = function (data) {
    var that = this;

    ko.mapping.fromJS(data, {}, this);

    this.selectedModelId = ko.observable();

    if (data && data.ModelId)
    {
        this.selectedModelId(data.ModelId);
    }

    this.selectModelSize = function (modelData, event) {
        that.selectedModelId(modelData.ModelId()); // for latency compensation

        $.post("/box/changeselectionmodel", {
            "ReplacedModelId": that.ModelId(),
            "ModelId": modelData.ModelId(),
            "PackId": modelData.PackId(),
            "ProductId": modelData.ProductId()
        },
        function (result) {
            ko.mapping.fromJS(result, selectionMapping, selectionModel);
        },
        'json');
    };
}

var selectionMapping = {
    'SelectedProducts': {
        create: function (options) {
            return new extSelectionModel(options.data);
        }
    }
};

$.getJSON("/Box/GetSubscription", function (data) {
    if (data) {
        selectionModel = ko.mapping.fromJS(data, selectionMapping);
        selectionModel.couponCode = ko.observable();
        selectionModel.applyCoupon = function () {
            $.post("/box/applycoupon", { code: selectionModel.couponCode() }, function (result) {
                ko.mapping.fromJS(result, selectionMapping, selectionModel);
            });
        };

        selectionModel.removeModel = function (data) {
            $.post("/box/removefromsubscription", { modelId: data.ModelId }, function (result) {
                ko.mapping.fromJS(result, selectionMapping, selectionModel);
            });
        };


        selectionModel.removePack = function (data) {
            $.post("/box/removepackfromsubscription", { packId: data.Pack.PackId }, function (result) {
                ko.mapping.fromJS(result, selectionMapping, selectionModel);
            });
        };

        selectionModel.validateCheckout = function (data) {
            //add validation here
            return true;
        };

        ko.applyBindings(selectionModel);
    }
});

$('.pack').popover('show');
setTimeout(function () { $('.pack').popover('hide'); }, 4200);