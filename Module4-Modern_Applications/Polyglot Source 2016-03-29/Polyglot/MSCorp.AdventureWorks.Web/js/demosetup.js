var app = angular.module("mainPageApp");


app.directive('fileUploader', function () {
    return {
        restrict: 'E',
        transclude: true,
        template: '<div><input type="file" multiple /><button ng-click="upload()">Upload</button></div>'
		+ '<ul><li ng-repeat="file in files">{{file.name}} - {{file.type}}</li></ul>',
        controller: function ($scope, $fileUpload) {
            $scope.notReady = true;
            $scope.upload = function () {
                $fileUpload.upload($scope.files);
            };
        },
        link: function ($scope, $element) {
            var fileInput = $element.find('input[type="file"]');
            fileInput.bind('change', function (e) {
                $scope.notReady = e.target.files.length == 0;
                $scope.files = [];
                for (i in e.target.files) {
                    if (typeof e.target.files[i] == 'object') $scope.files.push(e.target.files[i]);
                }
            });
        }
    }
});

app.service('$fileUpload', ['$http', function ($http) {
    this.upload = function (files) {
        var formData = new FormData();
        for (i in files) {
            formData.append('file_' + i, files[i]);
        }
        console.log(formData);
        $http({ method: 'POST', url: window.location + '/importdata/1', data: formData, headers: { 'Content-Type': undefined }, transformRequest: angular.identity })
            .error(function(data, status) {
                console.log(status);
                console.log(data);
                $("#upload-message").text("Content upload did not complete successfully, some records may have been imported.  Error was: " + data);
            })
            .success(function (data, status, headers, config) {
                console.log(status);
                console.log(data);
                if (status === 200) {
                    $("#upload-message").text("Content upload completed!");
                }
            });
        $("#upload-message").text("Beginning content upload.  Sit back and relax, this will take a short while...");

    }
}]);