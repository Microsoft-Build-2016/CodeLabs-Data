var app = angular.module("mainPageApp");


app.controller('ProductViewModel', function ($scope, $http, currentUserService, price) {
    var Product = function (productCode, name, description, colour, quantity, sizes, image, thumbImage, productAttributes, priceSet) {
        this.productCode = productCode;
        this.name = name;
        this.description = description;
        this.colour = colour;
        this.quantity = quantity;
        this.sizes = sizes;
        this.image = image;
        this.thumbImage = thumbImage;
        this.productAttributes = productAttributes;
        this.priceSet = priceSet;

        this.add = function () {
            this.quantity++;
        };
        this.subtract = function () {
            if (this.quantity <= 1) return;
            this.quantity--;
        };
    };

    var ProductSize = function (productSize) {
        this.value = productSize;
    };

    $scope.productDetails = {};
    $scope.productImage = {};
    $scope.productImageThumb = {};
    $scope.selectedSize = 0;
    $scope.enableAddToCart = false;
    $scope.successfulAddToCart = false;
    $scope.productReviews = [];
    $scope.productJson = rootUrl + 'Api/Search/Product/' + productId;
    $scope.productReviewJson = rootUrl + 'Api/ProductReview/' + productId;
    $scope.addReviewText = "";
    $scope.addReviewRating = 5;
    $scope.addButtonText = "Add to cart";

    $scope.loadProduct = function () {
        return $http.get($scope.productJson)
            .then(function (results) {
                var item = results.data;
                var sizes = [];
                $.each(item.ProductSizes, function (i, size) {
                    sizes.push(new ProductSize(size.Name));
                });

                currentUserService.promise.then(function() {
                var image = item.attachmentCollection.filter(function (attachment) { return !attachment.isThumbnail; })[0].media;
                var thumbImage = item.attachmentCollection.filter(function (attachment) { return attachment.isThumbnail; })[0].media;
                    var priceSet = new price.PriceSet(item.Prices, item.DiscountedPrices, item.Discount.PercentageValue, currentUserService.currentUser().PreferredCurrency.Code);

                $scope.productDetails = new Product(item.ProductCode, item.ProductName, item.ProductDescription, item.Color, 1, sizes, image, thumbImage, item.ProductAttributes, priceSet);
                $scope.selectedSize = $scope.productDetails.sizes[0];
            });
            });
    };
    $scope.loadReviewsFromData = function (data) {
        $scope.productReviews = data;
        data.forEach(function (item) {
            item.commentId = "comment" + item.id.replace('==', '');
        });
        $scope.addReviewText = "";
    };

    $scope.toggleComment = function (review) {
        var commentId = review.commentId;
        $(document.getElementById("button" + commentId)).toggle();
        $(document.getElementById("input" + commentId)).toggle();
        $(document.getElementById("text" + commentId)).focus();
    };



    $scope.loadReviews = function () {
        return $http.get($scope.productReviewJson)
            .then(function (results) {
                $scope.loadReviewsFromData(results.data);
            });
    };


    $scope.addToCart = function () {
        currentUserService.promise.then(function() {
        $scope.successfulAddToCart = false;
        var url = rootUrl + 'Api/Cart/Add';

        var data = {
            orderLine: { productDetails: $scope.productDetails, selectedSize: $scope.selectedSize, thumbImage: $scope.productDetails.thumbImage },
                customer: currentUserService.currentUser()
        };

        $http.post(url, data).success(function () {
            $scope.successfulAddToCart = true;
            $scope.addButtonText = "Added";
            $(".btn-purchase").addClass("item-added");
            $scope.cart.quantity += $scope.productDetails.quantity;
        });
        });
    };




    $scope.addReview = function () {
        if ($scope.addReviewText.length) {
            currentUserService.promise.then(function() {
            var reviewJson = {
                "ProductCode": $scope.productDetails.productCode,
                "ReviewText": $scope.addReviewText,
                "Rating": $("#AddReviewRating").rateit("value"),
                "ReviewDate": Date.now(),
                "ReviewingCustomer": {
                        "Name": currentUserService.currentUser().FirstName + " " + currentUserService.currentUser().LastName,
                        "CustomerKey": currentUserService.currentUser().CustomerKey
                },
            };


            $http.post(rootUrl + "Api/ProductReview/Add", reviewJson)
                    .success(function() {
                    $scope.loadReviews();
                    }).error(function() {
                    alert("Unable to add review");
                });
            });
        }
    };

    $scope.addComment = function (review) {
        var comment = $(document.getElementById("text" + review.commentId)).val();
        if (comment.length) {
            currentUserService.promise.then(function() {
            var data = {
                "ResponseText": comment,
                "CustomerResponding": {
                        "Name": currentUserService.currentUser().FirstName + " " + currentUserService.currentUser().LastName,
                        "CustomerKey": currentUserService.currentUser().CustomerKey
                },
            };
            var url = rootUrl + 'Api/ProductReview/Respond/' + review.id;
            $http.post(url, data)
                .success(function () {
                    $scope.loadReviews();
                }).error(function (data) {
                    alert('Unable to add response');
                });
            });
        }
    };

    $scope.initialisePage = function () {
        currentUserService.promise.then(function () {
            $scope.loadProduct();
        }).then(function () {
            $scope.loadReviews();
            $scope.enableAddToCart = true;
        });

    };


    $scope.initialisePage();
    $scope.$on('onRepeatLast', function (scope, element, attrs) {
        $('.rateit').rateit();
    });
});

