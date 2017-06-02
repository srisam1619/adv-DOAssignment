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



App.controller('reportprint_ctrl', ['$scope', '$rootScope', '$http', '$window', '$cookies','util_SERVICE','$location',

function ($scope, $rootScope, $http, $window, $cookies,US,$location) {
	
	$scope.user = angular.fromJson($cookies.get('UserData'));
	$scope.burl = angular.copy(US.Burl);
	
	
	$scope.docno = getQueryVariable("id");
	US.CreatePDF($scope.docno,$scope.user[0].CompanyCode).then(function (response) 
				{
					console.log(response);
					if(response.data[0].Result=="Success")
					{
						$scope.pdflink = $scope.burl+response.data[0].DisplayMessage;
						
						 printPdf($scope.pdflink);
	
	
					}
					else
					{
						alert(response.data[0].DisplayMessage);
					}
				});
	
	
} ]);

printPdf = function (url) {
  var iframe = this._printIframe;
  if (!this._printIframe) {
    iframe = this._printIframe = document.createElement('iframe');
    document.body.appendChild(iframe);

    iframe.style.display = 'none';
    iframe.onload = function() {
      setTimeout(function() {
        iframe.focus();
        iframe.contentWindow.print();
      }, 1);
    };
  }

  iframe.src = url;
}