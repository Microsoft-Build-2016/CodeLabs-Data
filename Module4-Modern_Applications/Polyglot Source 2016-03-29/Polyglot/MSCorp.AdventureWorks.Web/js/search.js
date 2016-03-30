var app = angular.module("mainPageApp");

app.controller('SearchViewModel', function ($scope, $http, currentUserService, price) {
    $scope.products = [];
    $scope.refiners = [];
    $scope.hasFinished = false;
    $scope.isRefining = false;
    $scope.fromDocDb = false;
    $scope.searchResultMessage = searchText;
    //$scope.currentUser = {};
    $scope.exchangeRates = [];
    $scope.requestUrl = "";
    $scope.drilldownParameters = [];
    $scope.searchText = searchText;

    $scope.sortFields = [{ name: 'Best Match', value: 'score' }, { name: 'Price', value: 'Price' }, { name: 'Name', value: 'Name' }];
    $scope.sortField = $scope.sortFields[0];

    $scope.sortDirections = [{ name: 'Ascending', value: 'asc' }, { name: 'Descending', value: 'desc' }];
    $scope.sortDirection = $scope.sortDirections[0];

    $scope.resultCount = function () {
        return $scope.products.length;
    };

    $scope.searchSubmit = function () {
        if ($scope.searchText) {
            window.location.href = rootUrl + 'Search/' + encodeURI($scope.searchText);
        }
    };
    var ProductSummary = function (productId, productCode, name, priceSet, image) {
        this.productId = productId;
        this.productCode = productCode;
        this.name = name;
        this.priceSet = priceSet;
        this.image = image;
    };

    $scope.loadProducts = function () {

        $scope.requestUrl = rootUrl + 'Api/Search/Products/';
        if (searchAttribute) {
            $scope.requestUrl = rootUrl + 'Api/Search/RelatedProducts/' + encodeURI(searchAttribute) + '/';
            $scope.searchResultMessage = searchAttribute + " matching " + searchText;
            $scope.fromDocDb = true;
        }
        $scope.requestUrl += encodeURI(searchText);

        return $http.get($scope.requestUrl)
            .then(function (results) {
                $scope.hasFinished = true;
                if ($scope.fromDocDb) {
                    $scope.products = processProducts(results.data.value);
                } else {
                    $scope.products = processProducts(results.data.Results);
                }
                $scope.refiners = [];

                for (propertyName in results.data.Facets) {
                    var filters = [];
                    var filterResponses = results.data.Facets[propertyName];

                    $.each(filterResponses, function (index, filter) {
                        if (propertyName === 'SizeFacet') {
                            //todo: KM - can we rework the SizeFacet stuff, this is pretty gross.
                            var replacmentToken = '#$%^&*(zz';
                            var item = angular.fromJson(filter.Value.replace('""', replacmentToken + '"'));
                            filter.Value = item.name.replace(replacmentToken, '"');
                        } 
                        filters.push(new Filter(filter.Value, filter.Count));
                    });
                    var refinerName = propertyName;
                    var isCollectionRefiner = false;
                    if (propertyName === 'SizeFacet') {
                        refinerName = 'Size';
                        isCollectionRefiner = true;
                    }

                    var refiner = new Refiner(refinerName, isCollectionRefiner, filters);
                    $scope.refiners.push(refiner);
                }
            });
    };

    function processProducts(items) {
        var products = [];
        $.each(items, function (index, item) {
            var exchangeRate = {};
            var priceSet = {};

            if ($scope.fromDocDb) {
                var thumbImage = item.attachmentCollection.filter(function (attachment) { return attachment.isThumbnail; })[0].media;;
                exchangeRate = $scope.exchangeRates.filter(function (rate) { return rate.isForCurrency(item.Price.Currency.Code); })[0];
                priceSet = exchangeRate.calculatePrices(item.Price.Amount, item.Discount.PercentageValue, currentUserService.currentUser().PreferredCurrency.Code);
                products.push(new ProductSummary(item.id, item.ProductCode, item.ProductName, priceSet, thumbImage));
            } else {
                var product = item.Document;
                exchangeRate = $scope.exchangeRates.filter(function (rate) { return rate.isForCurrency(product.CurrencyCode); })[0];
                priceSet = exchangeRate.calculatePrices(product.Price, product.Discount, currentUserService.currentUser().PreferredCurrency.Code);
                products.push(new ProductSummary(product.key, product.ProductCode, product.Name, priceSet, product.ThumbImageUrl));
            }
        });
        return products;
    }

    $scope.refineProducts = function () {
        $scope.isRefining = true;

        $scope.requestUrl = rootUrl + 'Api/Search/Products/';
        if (searchAttribute) {
            $scope.requestUrl = rootUrl + 'Api/Search/RelatedProducts/' + encodeURI(searchAttribute) + '/';
            $scope.searchText += " related to " + searchAttribute;
            $scope.fromDocDb = true;
        }
        $scope.requestUrl += encodeURI(searchText);


        $scope.drilldownParameters = [];
        var activeRefiners = $scope.refiners.filter(function (refiner) { return refiner.hasSelectedFilters(); });
        $.each(activeRefiners, function (index, item) {
            $scope.drilldownParameters.push(item.createFilterQuery());
        });

        var orderByQuery = $scope.createOrderByQuery();
        if ($scope.drilldownParameters.length > 0) {
            var drillDownQuery = $scope.createDrillDownQuery();
            $scope.requestUrl += '/Drilldown/?query=' + drillDownQuery + '&orderBy=' + orderByQuery;
        } else {
            if (!$scope.fromDocDb) {
                $scope.requestUrl += '/?orderBy=' + orderByQuery;
            }
        }

        $http.get($scope.requestUrl)
            .then(function (results) {
                $scope.products = processProducts(results.data.Results);
                $scope.isRefining = false;
            });
    }

    $scope.createDrillDownQuery = function () {
        var query = '';
        var groupCount = $scope.drilldownParameters.length;
        $.each($scope.drilldownParameters, function (groupIndex, group) {
            query += '(';
            var filterCount = group.filters.length;
            $.each(group.filters, function (index, filter) {
                if (group.isCollection) {
                    query += group.name + '/any(i:i eq \'' + filter + '\')';
                } else {
                    query += group.name + ' eq \'' + filter + '\'';
                }
                if (index < filterCount - 1) {
                    query += ' or ';
                }
            });

            query += ')';
            if (groupIndex < groupCount - 1) {
                query += ' and ';
            }
        });

        return query;
    };

    $scope.createOrderByQuery = function () {
        if ($scope.sortField.value === 'score') {
            return '';
        }
        var query = $scope.sortField.value + ' ' + $scope.sortDirection.value;
        return query;
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

var Refiner = function (name, isCollectionRefiner, filters) {
    var self = this;
    self.name = name;
    self.isCollectionRefiner = isCollectionRefiner;
    self.filters = filters;
    self.hasFilters = function () {
        return self.filters.length > 0;
    };

    self.selectedFilters = function () {
        return self.filters.filter(function (item) { return item.selected; });
    };

    self.hasSelectedFilters = function () {
        return self.selectedFilters().length > 0;
    };

    self.createFilterQuery = function () {
        var selectedFilters = self.selectedFilters().map(function (item) { return item.name; });
        return { name: name, isCollection: isCollectionRefiner, filters: selectedFilters };
    };
};

var Filter = function (name, count) {
    this.name = name;
    this.count = count;
    this.selected = false;
};