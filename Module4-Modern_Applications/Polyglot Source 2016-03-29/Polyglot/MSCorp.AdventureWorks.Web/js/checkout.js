var app = angular.module("mainPageApp");

app.controller('CheckoutViewModel', function ($scope, $http, currentUserService, price) {

    $scope.cart = {};
    $scope.disablePurchaseButton = false;
    $scope.purchaseWasSuccessful = false;
    $scope.exchangeRates = [];
    $scope.hasLoaded = false;
    $scope.currentUser = {};

    var Cart = function (shipping, gst, items) {
        var self = this;
        self.shipping = shipping.toFixed(2);
        self.gst = gst;
        self.items = items;
        self.count = function () {
            return self.items.length;
        };
        self.subTotal = function () {
            var subTotal = 0;
            $.each(self.items, function (i, value) {
                subTotal = subTotal + value.totalCost();
            });
            return subTotal.toFixed(2);
        };
        self.calculatedGstCost = function () {
            return (self.subTotal() * self.gst).toFixed(2);
        };
        self.grandTotal = function () {
            return (+self.subTotal() + +self.shipping).toFixed(2);
        };
    };

    var CheckoutItem = function (productCode, image, name, description, quantity, size, priceSet) {
        var self = this;
        self.productCode = productCode;
        self.image = image;
        self.name = name;
        self.description = description;
        self.quantity = quantity;
        self.size = size;
        self.priceSet = priceSet;
        self.totalCost = function () {
            return self.quantity * self.priceSet.displayDiscountedPrice.Amount;
        };
    };

    $scope.loadCart = function () {
        currentUserService.promise.then(function () {
            $scope.currentUser = currentUserService.currentUser();
            $http.get(rootUrl + 'Api/Cart/' + currentUserService.currentUser().CustomerKey)
            .then(function (results) {
                var shipping = 25.00;
                var tax = 0.1;
                var cartItems = [];
                if (results.data.orderLines) {
                    $.each(results.data.orderLines, function (i, value) {

                        //Check to make sure there is a size against the product (Temp)
                        value.selectedSize = value.selectedSize || {};
                        value.selectedSize.value = value.selectedSize.value || '';

                        cartItems.push(new CheckoutItem(
                            value.productDetails.productCode,
                            value.thumbImage,
                            value.productDetails.name,
                            value.productDetails.description,
                            value.productDetails.quantity,
                            value.selectedSize.value,
                            value.productDetails.priceSet));
                    });
                }

            $scope.cart = new Cart(shipping, tax, cartItems);
            $scope.showPurchaseButton = cartItems;
            $scope.hasLoaded = true;
            });
        });
    };

    $scope.purchase = function () {
        $scope.disablePurchaseButton = true;
        var url = rootUrl + 'Api/Cart/PlaceOrder';
        var data = {
            order: $scope.cart,
            customer: $scope.currentUser
        };

        $http({ method: 'POST', url: url, data: data })
            .success(function() {
                console.log('Success!');
                $scope.purchaseWasSuccessful = true;
            })
            .error(function(err) {
                $scope.disablePurchaseButton = false;
                console.log(err);
            });

    };

    $scope.initialisePage = function () {
        currentUserService.promise.then(function () {
            price.promise.then(function () {
                $scope.exchangeRates = price.loadRates();
                $scope.loadCart();
            });
        });
    };

    $scope.initialisePage();

});