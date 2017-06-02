App.controller('signin', ['$scope', '$rootScope', '$http', '$window', '$cookies',

function ($scope, $rootScope, $http, $window, $cookies) {
    // $scope.items = Data;
    $scope.userId = "";
    $scope.password = "";
    $cookies.put('MenuInfo', "");
    $cookies.put('UserData', "");
    $cookies.put('Islogin', "false");
	
	//var url = "http://54.251.51.69:3890/DOService.asmx/";
	var url = "http://192.168.0.38:83/DOService.asmx/";
	$scope.company = "ADV_DO";
	
	$scope.loadcompany = function () {
		
		

        var data = {"sUserName" : $scope.userId, "sPassword" : $scope.password, "sCompany" : $scope.company}

        var config = {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded;charset=utf-8;'
            }
        }

        var parms = JSON.stringify(data);
        $http.post(url+"GetCompanyList", "sJsonInput=" + "", config)
   .then(
       function (response) {
           // success callback
           console.log(response.data);
           if (response.data !="") {
             $scope.companylist = response.data;
			 $scope.company = $scope.companylist[0].U_DBName;
           }
            else
               $scope.companylist = [];
       },
       function (response) {
           // failure callback

       }
    );

    
		
		
	}

    $scope.checklogin = function () {

        var data = {"sUserName" : $scope.userId, "sPassword" : $scope.password, "sCompany" : $scope.company}

        var config = {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded;charset=utf-8;'
            }
        }

        var parms = JSON.stringify(data);
        $http.post(url+"LoginValidation", "sJsonInput=" + parms, config)
   .then(
       function (response) {
           // success callback
           console.log(response.data);
           if (response.data[0].UserId != "") {
               //$cookies.put('MenuInfo', JSON.stringify(response.data.MenuInfo));
               $cookies.put('UserData', JSON.stringify(response.data));
               $cookies.put('Islogin', "true");
               window.location = "Postal_Zone.html";
           }
            else
               alert(response.data[0].Message);
       },
       function (response) {
           // failure callback

       }
    );

    }


$scope.loadcompany();
} ]);
