var app = angular.module("mainPageApp");

app.controller('CategoryViewModel', function ($scope, $http) {
    $scope.products = [];
    $scope.facets = [];

    $scope.loadProducts = function (val) {
        return $http.get(rootUrl + 'Api/Search/ProductsByCategory/' + encodeURI(categoryName))
            .then(function (results) {
                //todo: KM - remove the following line once we have images coming back.
                //$.each(results.data.value, function (i, value) { value.data.Image = imageUrl + 's-' + value.key + '.gif'; });
                $scope.products = results.data.Results;
            });
    };
    $scope.loadProducts();


});