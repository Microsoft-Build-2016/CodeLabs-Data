var app = angular.module("mainPageApp");

app.controller('OrderHistoryController', function ($scope, $http, currentUserService) {
    $scope.hasLoaded = false;
    $scope.currentUser = {};
    $scope.orderItems = [];

    $scope.loadHistory = function () {
            $http.get(rootUrl + 'Api/Customer/Orders/' + $scope.currentUser.CustomerKey)
            .then(function (results) {
                if (results.data) {
                    $scope.orderItems = results.data.sort(function (item) { return item.OrderDate; }).reverse();
                }
                $scope.hasLoaded = true;
            });
    };

    $scope.initialisePage = function () {
        currentUserService.promise.then(function () {
            $scope.currentUser = currentUserService.currentUser();
            $scope.loadHistory();
        });
    };

    $scope.initialisePage();

});

app.controller('OrderHistoryDetailController', function ($scope, $http, $q, currentUserService) {
    $scope.hasLoaded = false;
    $scope.currentUser = {};
    $scope.productDetails = {};

    $scope.loadProductDetails = function () {
        $.each(products, function (index, productCode) {
            $http.get(rootUrl + 'Api/Search/Product/' + productCode).then(function (result) {
                result.data.thumbnailUrl = result.data.attachmentCollection.filter(function (item) { return item.isThumbnail; })[0].media;
                $scope.productDetails[productCode] = result.data;
            });
        });
    }

    $scope.initialisePage = function () {
        $scope.loadProductDetails();
        currentUserService.promise.then(function () {
            $scope.currentUser = currentUserService.currentUser();
            $scope.hasLoaded = true;
        });
    };

    $scope.initialisePage();
});