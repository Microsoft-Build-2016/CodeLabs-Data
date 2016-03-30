var app = angular.module("mainPageApp");

app.controller('DiscountController', function ($scope, $http, $q, currentUserService, price) {
    $scope.products = [];
    $scope.hasFinished = false;
    $scope.exchangeRates = [];
    $scope.requestUrl = "";
    $scope.searchResultMessage = "";
    $scope.selectedAttribute;
    $scope.selectedAttributeValue;
    $scope.availableAttributes = [];
    $scope.newDiscount = 0;
    $scope.resultCount = function() {
        return $scope.products.length;
    };

    var ProductSummary = function (productId, productCode, etag, name, priceSet, attributes, image) {
        this.productId = productId;
        this.productCode = productCode;
        this.etag = etag;
        this.name = name;
        this.priceSet = priceSet;
        this.attributes = attributes;
        this.image = image;
    };

    var ProductAttributeFilter = function(attributeName) {
        
        this.attributeName = attributeName;
        this.options = [];
        this.addOption = function addOption(value) {
            if (this.options.indexOf(value) == -1) this.options.push(value);
        };
    };

    $scope.loadProductAttributes = function () {
        $scope.requestUrl = rootUrl + 'Api/Search/AllProductAttributes/';
        return $http.get($scope.requestUrl)
            .then(function (results) {
                $scope.refiners = [];
                loadAttributeFacets(results.data);
        });
    };

    function processProducts(items) {
        var products = [];
        $.each(items, function (index, item) {
            var exchangeRate = {};
            var priceSet = {};

            exchangeRate = $scope.exchangeRates.filter(function (rate) { return rate.isForCurrency(item.Price.Currency.Code); })[0];
            priceSet = exchangeRate.calculatePrices(item.Price.Amount, item.Discount.PercentageValue, currentUserService.currentUser().PreferredCurrency.Code);

            var image = item.attachmentCollection.filter(function (attachment) { return !attachment.isThumbnail; })[0].media;
            products.push(new ProductSummary(item.id, item.ProductCode, item._etag, item.ProductName, priceSet, item.ProductAttributes, image));
        });

        return products;
    }

    function loadAttributeFacets(attributes) {

        $.each(attributes, function (index, attribute) {
            for (var property in attribute.ProductAttributes) {
                var attributeFilters = $scope.availableAttributes.filter(function(attr) { return attr.attributeName === property; });
                var filter = attributeFilters[0];
                if (attributeFilters.length === 0) {
                    filter = new ProductAttributeFilter(property);
                    $scope.availableAttributes.push(filter);
                }

                filter.addOption(attribute.ProductAttributes[property]);
            }
        });
    }

    $scope.filterProducts = function() {
        $scope.requestUrl = rootUrl + 'Api/Search/RelatedProducts/' + encodeURI($scope.selectedAttribute.attributeName) + '/' + encodeURI($scope.selectedAttributeValue);

        return $http.get($scope.requestUrl)
            .then(function (results) {
                $scope.hasFinished = true;
                $scope.products = processProducts(results.data.value);
                $scope.newDiscount = 0;
                $scope.searchResultMessage = $scope.selectedAttribute.attributeName + " matching " + $scope.selectedAttributeValue;
            });
    }

    $scope.previewDiscount = function() {
        $.each($scope.products, function(index, product) {
            product.priceSet.setDiscount($scope.newDiscount);
        });
    }

    $scope.applyDiscount = function() {
        var url = rootUrl + 'Admin/Product/Discount';
        var data = {
            discount: $scope.newDiscount,
            products: $scope.products.map(function (item) { return { id: item.productId, productCode: item.productCode, etag: item.etag }; })
        };

        $http({ method: 'POST', url: url, data: data })
            .success(function () {
                console.log('Success!');
                alert('Discount has been applied successfully.');
                $scope.filterProducts();
            })
            .error(function(err) {
                console.log('Error: ' + err);
                alert('Unable to apply discount.  One or more products may have been modified, please refresh this view to try again.');
            });
    }

    $scope.initialisePage = function () {
        currentUserService.promise.then(function () {
            price.promise.then(function () {
                $scope.exchangeRates = price.loadRates();
                $scope.loadProductAttributes().then(function() {
                    $scope.selectedAttribute = $scope.availableAttributes[0];
                    $scope.selectedAttributeValue = $scope.availableAttributes[0].options[0];
                    $scope.filterProducts();

                });
            });
        });
    };

    $scope.initialisePage();

});

var Refiner = function (name, isCollectionRefiner, filters) {
    var self = this;
    self.name = name;
    self.isCollectionRefiner = isCollectionRefiner;
    self.filters = filters;
    self.hasFilters = function() {
        return self.filters.length > 0;
    };

    self.selectedFilters = function () {
        return self.filters.filter(function(item) { return item.selected; });
    };

    self.hasSelectedFilters = function () {
        return self.selectedFilters().length > 0;
    };

    self.createFilterQuery = function () {
        var selectedFilters = self.selectedFilters().map(function(item) { return item.name; });
        return { name: name, isCollection: isCollectionRefiner, filters: selectedFilters };
    };
};

var Filter = function (name, count) {
    this.name = name;
    this.count = count;
    this.selected = false;
};