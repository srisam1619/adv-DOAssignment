App.controller('main', ['$scope', '$rootScope', '$http', '$window', '$cookies',

function ($scope, $rootScope, $http, $window, $cookies) {
    // $scope.items = Data;
    $scope.userId = "";
    $scope.password = "";

    if ($cookies.get('Islogin') == "true") {
       // alert("ss");
    }
    else {
        window.location = "login.aspx";
    }
    $scope.MenuInfo = $cookies.get('MenuInfo');
    $scope.UserData = angular.fromJson($cookies.get('UserData'));


    $scope.username = "Admin";
    $scope.user_role = "Administrator";


} ]);
