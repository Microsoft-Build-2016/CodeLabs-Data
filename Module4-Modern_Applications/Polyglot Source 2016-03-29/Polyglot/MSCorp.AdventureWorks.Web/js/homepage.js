var app = angular.module("mainPageApp");

app.controller('ProductCategoriesController', function ($scope, $http, $q, currentUserService, price) {
    $scope.products = [];
    $scope.productCategories = [];
    $scope.productLimit = 4;
    $scope.exchangeRates = [];

    price.promise.then(function () {
        $scope.exchangeRates = price.loadRates();
    });

    var ProductSummary = function (productCode, name, category, priceSet, image) {
        this.productCode = productCode;
        this.name = name;
        this.category = category;
        this.image = image;
        this.priceSet = priceSet;
    };

    $scope.lowestCost = function (category) {
        var productsForCategory = $scope.products.filter(function (item) { return item.category === category; });
        if (productsForCategory && !productsForCategory.length) return 0;
        var lowestPrice;
        $.each(productsForCategory, function (index, item) {
            var itemPrice = item.priceSet.displayDiscountedPrice.Amount;
            if (!lowestPrice) {
                lowestPrice = itemPrice;
            } else if (itemPrice < lowestPrice) {
                lowestPrice = itemPrice;
            }
        });
        return lowestPrice;
    };

    var processItems = function (items) {

        var deferred = $q.defer();

        currentUserService.promise.then(function () {

            var products = [];
            $.each(items, function (index, value) {
                var item = value.Document;
                var exchangeRate = $scope.exchangeRates.filter(function (rate) { return rate.isForCurrency(item.CurrencyCode); })[0];
                var priceSet = exchangeRate.calculatePrices(item.Price, item.Discount, currentUserService.currentUser().PreferredCurrency.Code);
                products.push(new ProductSummary(item.ProductCode, item.Name, item.ProductCategory, priceSet, item.ThumbImageUrl));
            });
            deferred.resolve(products);
        });
        return deferred.promise;


    };

    $scope.loadProducts = function () {

        $http.get(rootUrl + 'Api/Search/ProductsByCategory/Gear/' + $scope.productLimit)
            .then(function (results) {
                $scope.productCategories = $scope.productCategories.concat(results.data.Facets.ProductCategory);
                var items = results.data.Results;
                processItems(items).then(function (result) {
                    $scope.products = $scope.products.concat(result);
                });
            });

        $http.get(rootUrl + 'Api/Search/ProductsByCategory/Rugged/' + $scope.productLimit)
            .then(function (results) {
                $scope.productCategories = $scope.productCategories.concat(results.data.Facets.ProductCategory);
                var items = results.data.Results;
                processItems(items).then(function (result) {
                    $scope.products = $scope.products.concat(result);
                });
            });

        $http.get(rootUrl + 'Api/Search/ProductsByCategory/Smooth/' + $scope.productLimit)
            .then(function (results) {
                $scope.productCategories = $scope.productCategories.concat(results.data.Facets.ProductCategory);
                var items = results.data.Results;
                processItems(items).then(function (result) {
                    $scope.products = $scope.products.concat(result);
                });
            });

        var first = true;   //  open the first accordion by default
        $scope.productCategories.forEach(function (category) {
            category.open = first;
            first = false;
        });
    };

    $scope.categoryMatcher = function (category) {
        return function (product) {
            return product.category === category.Value;
        };
    };

    $scope.initialisePage = function () {
        currentUserService.promise.then(function () {
            price.promise.then(function () {
                $scope.exchangeRates = price.loadRates();
                $scope.loadProducts();
            });
        });
    };

    $scope.initialisePage();

});
