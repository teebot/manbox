/// <reference path="jquery-1.9.1.js" />
/// <reference path="knockout-2.2.1.js" />
/// <reference path="knockout.mapping-latest.debug.js" />
/// <reference path="sammy-0.7.4.js" />

// Subscription Cart

var selectionModel;

var extSelectionModel = function (data) {
    ko.mapping.fromJS(data, {}, this);
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

        ko.applyBindings(selectionModel, $("#subscr-selection")[0]);
    }
});

$(document).ready(function () {
    $('.gift-address-notice').tooltip();
});