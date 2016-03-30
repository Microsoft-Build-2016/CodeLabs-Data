'use strict';
//todo: KM - this should live somewhere more reasonable.
new UISearch(document.getElementById('sb-search'));

var mainPageApp = angular.module('mainPageApp', ['ui.bootstrap', 'ngSanitize']);

//  use the on-last-repeat attribute on a ng-repeat element to trigger a post render function.
//  you can then implement the following to perform any code once the render has completed:
//      $scope.$on('onRepeatLast', function (scope, element, attrs) {
//          DoPostRenderUpdatesHere();
//      });
//  see http://www.nodewiz.biz/angular-js-final-callback-after-ng-repeat/ for details
mainPageApp.directive("onLastRepeat", function () {
    return function (scope, element, attrs) {
        if (scope.$last)
            setTimeout(function () {
                scope.$emit('onRepeatLast', element, attrs);
            }, 1);
    };
});

mainPageApp.service('currentUserService', function ($http) {
    var myUser = null;
    var userId = ($.cookie('currentUserId') || '');
    var url = rootUrl + 'Api/Customer/Get/' + userId;
    if (userId == '') {
        url = rootUrl + 'Api/Customer/GetFirstCustomer/';
    }

    var promise = $http.get(url)
        .then(function(results) {
            myUser = results.data;
            $.cookie('currentUserId', myUser.id, { path: '/' });
        });

    return {
        promise: promise,
        loadUser: function () {
            return myUser;
        },
        currentUser: function () {
            return myUser;
        },
        currentUserWip: function () {
            return promise.then(function () {
                alert("internal " + JSON.stringify(myUser));
                return myUser;
            });
        },
        loadUserById: function (userId) {
            $http.get(rootUrl + 'Api/Customer/Get/' + userId)
                .then(function (results) {
                    myUser = results.data;
                    $.cookie('currentUserId', myUser.id, { path: '/' });
                    if (location.href.toLowerCase().indexOf('/orderhistory/detail') != -1) {
                        location.replace(rootUrl + "OrderHistory");
                    } else {
                        location.reload();//  reload so that the cart/pricing/etc get reloaded correctly
                    }
                });
        }
    };
});

mainPageApp.service('cart', function ($http, currentUserService) {
    var myCartSummary = { quantity: 0, lineCount: 0 };


    var promise = currentUserService.promise.then(function () {
        $http.get(rootUrl + 'Api/Cart/' + currentUserService.currentUser().CustomerKey)
            .then(function (results) {
                myCartSummary.quantity = 0;
                myCartSummary.lineCount = 0;
                if (results.data.orderLines) {
                    $.each(results.data.orderLines, function (i, value) {
                        myCartSummary.lineCount++;
                        myCartSummary.quantity += value.productDetails.quantity;
                    });
                };
            });
    });

    return {
        promise: promise,
        loadCartSummary: function () {
            return myCartSummary;
        }
    };
});

mainPageApp.service('price', function ($http) {
    var rates = [];

    var PriceSet = function (prices, discountedPrices, discountPercentage, defaultCurrency) {
        this.prices = prices;
        this.discountedPrices = discountedPrices;
        this.isDiscounted = discountPercentage > 0;
        this.discountPercentage = discountPercentage;
        this.displayPrice = {};
        this.displayDiscountedPrice = {};

        this.setCurrency = function (currencyCode) {
            this.displayPrice = prices.filter(function (item) { return item.Currency.Code === currencyCode; })[0];
            this.displayDiscountedPrice = discountedPrices.filter(function (item) { return item.Currency.Code === currencyCode; })[0];
        };

        this.setDiscount = function (newDiscount) {
            this.isDiscounted = newDiscount > 0;
            $.each(this.discountedPrices, function(index, discountedPrice) {
                var rrp = prices.filter(function (item) { return item.Currency.Code === discountedPrice.Currency.Code; })[0];
                var discountedAmount = (rrp.Amount.toFixed(2) * (100-newDiscount))/100;
                discountedPrice.Amount = discountedAmount;
            });
        };

        this.setCurrency(defaultCurrency);
    };

    var ExchangeRate = function (currency, rates) {
        this.currency = currency;
        this.rates = rates;

        this.isForCurrency = function (currencyCode) {
            return currencyCode === currency.Code;
        };

        this.calculatePrices = function (amount, discount, defaultDisplayCurrency) {
            var prices = [];
            var discountedPrices = [];

            // Always add the local rate.
            var localRate = { Currency: currency, Rate: 1 };
            rates.push(localRate);

            $.each(rates, function (index, rate) {
                var foreignPrice = calculateForeignCurrencyPrices(rate, amount);
                var foreignDiscountedPrice = calculateForeignCurrencyPrices(rate, amount, discount);
                prices.push(foreignPrice);
                discountedPrices.push(foreignDiscountedPrice);
            });

            return new PriceSet(prices, discountedPrices, discount, defaultDisplayCurrency);
        };

        function calculateForeignCurrencyPrices(foreignRate, price, discount) {
            var foreignPrice = price * foreignRate.Rate;
            if (discount) {
                foreignPrice = (+foreignPrice.toFixed(2) * (100 - discount)) / 100;
            }

            return { Currency: foreignRate.Currency, Amount: +foreignPrice.toFixed(2) };
        }
    };


    var promise = $http.get(rootUrl + 'Api/Search/Currencies')
        .then(function (results) {
            $.each(results.data, function (index, value) {
                var exchangeRate = new ExchangeRate(value.Currency, value.Rates);
                rates.push(exchangeRate);
            });
        });

    return {
        promise: promise,
        PriceSet: PriceSet,
        loadRates: function () {
            return rates;
        }
    };
});




mainPageApp.controller('MainPageViewModel', function ($scope, $http, $q, currentUserService, price, cart) {
    var Suggestion = function (text, productCode, comment) {
        this.text = text;
        this.productCode = productCode;
        this.comment = comment;
    };
    $scope.today = Date.today;
    $scope.cart = {};
    $scope.currentUser = {};
    $scope.exchangeRates = [];
    $scope.allCustomers = [];

    currentUserService.promise.then(function () {
        $scope.currentUser = currentUserService.currentUser();
    });

    price.promise.then(function () {
        $scope.exchangeRates = price.loadRates();
    });

    cart.promise.then(function () {
        $scope.cart = cart.loadCartSummary();
    });

    $scope.searchSubmit = function () {
        if ($scope.searchText) {
            window.location.href = rootUrl + 'Search/' + encodeURI($scope.searchText);
        }
    };

    $scope.loadProduct = function (item) {
        if (item.productCode === "") {
            $scope.searchSubmit();
        } else {
            window.location.href = rootUrl + 'Product/' + encodeURI(item.productCode);
        }
    };

    $scope.searchProducts = function(val) {
        var deferred = $q.defer();

        $http.get(rootUrl + 'Api/Search/Suggest/' + val).then(function(result) {
            var suggestions = [];
            $.each(result.data.Results, function(index, item) {
                suggestions.push(new Suggestion(item.Text, item.Document.ProductCode, ''));
            });
            deferred.resolve(suggestions);
        });
        return deferred.promise;
    };

    $scope.searchComments = function(val) {
        var deferred = $q.defer();
        $http.get(rootUrl + 'Api/Search/Comments/' + val).then(function (results) {
            var suggestions = [];
            $.each(results.data.Results, function (index, item) {
                var comment = '';
                if (item.Highlights && item.Highlights.Text) {
                    comment = item.Highlights.Text[0];
                }
                suggestions.push(new Suggestion(item.Document.ProductCode, item.Document.ProductCode, comment || ''));
            });
            deferred.resolve(suggestions);
        });
        return deferred.promise;
    };

    $scope.getAutoCompleteSuggestions = function (val) {
        var deferred = $q.defer();

        if (val && val.length > 2) {
            $q.all([$scope.searchProducts(val), $scope.searchComments(val)]).then(function (result) {
                var suggestions = [new Suggestion(val, "", "")];
                $.each(result, function (collectionIndex, collection) {
                    $.each(collection, function (index, inboundSuggestion) {
                        var existingSuggestion = suggestions.filter(function (sug) { return sug.productCode === inboundSuggestion.productCode; });

                        if (existingSuggestion.length == 0) {
                            suggestions.push(inboundSuggestion);
                        } else if (existingSuggestion[0].comment === '') {
                            existingSuggestion[0].comment = inboundSuggestion.comment || '';
                        }
                    });
                });
                deferred.resolve(suggestions);
            });
        }
        return deferred.promise;
    };


    $scope.setCurrentUser = function (customer) {
        currentUserService.loadUserById(customer.id);   //this will cause a page reload
        //$scope.currentUser = currentUserService.currentUser();  // so this is redundant
    };

    $scope.getAllCustomers = function () {
        $http.get(rootUrl + 'Api/Customer/GetAllCustomers').then(function (result) {
            $scope.allCustomers = result.data;
        });
    };
    $scope.getAllCustomers();
});
