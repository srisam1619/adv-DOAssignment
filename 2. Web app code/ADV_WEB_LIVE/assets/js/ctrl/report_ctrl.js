function getQueryVariable(variable) {
  var query = window.location.search.substring(1);
  var vars = query.split("&");
  for (var i=0;i<vars.length;i++) {
    var pair = vars[i].split("=");
    if (pair[0] == variable) {
      return pair[1];
    }
  } 
  alert('Query Variable ' + variable + ' not found');
}



App.controller('report_ctrl', ['$scope', '$rootScope', '$http', '$window', '$cookies','util_SERVICE','$location',

function ($scope, $rootScope, $http, $window, $cookies,US,$location) {
	
	$scope.user = angular.fromJson($cookies.get('UserData'));
	$scope.burl = angular.copy(US.Burl);
	
	
	$scope.docno = getQueryVariable("id");
	US.CreatePDF($scope.docno,$scope.user[0].CompanyCode).then(function (response) 
				{
					console.log(response);
					if(response.data[0].Result=="Success")
					{
						$scope.URL = $scope.burl+response.data[0].DisplayMessage;
						console.log($scope.URL);
						 document.getElementById('myIframe').src =$scope.URL;
					}
					else
					{
						alert(response.data[0].DisplayMessage);
					}
				});
	
	
} ]);