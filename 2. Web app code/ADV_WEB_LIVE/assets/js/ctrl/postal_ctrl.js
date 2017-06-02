App.controller('postal_ctrl', ['$scope', '$rootScope', '$http', '$window', '$cookies','util_SERVICE','$location',

function ($scope, $rootScope, $http, $window, $cookies,US,$location) {
	US.islogin();
    $scope.savelable = "Save";
    $scope.user = angular.fromJson($cookies.get('UserData'));
	
	
	
	 
	

$scope.getdriver = function()
{
    US.GetDrivers($scope.user[0].CompanyCode).then(function (response) 
	{
		$scope.Drivers = response.data;
		
	});
}
$scope.getdriver();
	
	 $scope.getteammaster = function()
	 {
	 US.GetTeamMaster($scope.user[0].CompanyCode).then(function (response) { 
																 
																 $scope.TeamMaster = $scope.addnewdriver(response.data);
																 $scope.TeamMasterC = angular.copy($scope.TeamMaster);
		  if($scope.checkDefaultDriver())
		  {
			  $scope.updatebtn = true;
			  $scope.savebtn = false;
			  $scope.bulkupdatebtn = true;
		  }
		  else
		  {
			  $scope.updatebtn = false;
			  $scope.savebtn = true;
			  $scope.bulkupdatebtn = false;
		  }});
	 }
	 
	 $scope.getteammaster();
	
	$scope.addnewdriver = function(data)
	{
		var rdata = [];
	  for (var i = 0; i < data.length; i++) {
		  data[i].NewDriverId=angular.copy(data[i].DefaultDriverId);
	  }
	  return data;
	  
	  
	}
$scope.update = function()
{

	if($scope.driver1== undefined || $scope.driver2==undefined)
	{
		alert("Kindly Select Original and New Driver!");
	}
	else
	{
	US.UpdateBulkPostalZoneMaster($scope.driver1,$scope.driver2,$scope.user[0].CompanyCode).then(function (response) {console.log(response);alert(response.data[0].DisplayMessage);$scope.getteammaster();});
	}
}

$scope.save = function()
{
	var data = $scope.getsavedata($scope.TeamMaster)
	//console.log(data);
	if($scope.valsavedata())
	{
	US.SavePostalZoneMaster(data,$scope.user[0].CompanyCode).then(function (response) {alert(response.data[0].DisplayMessage);$scope.getteammaster();});
	}
	else
	{
		alert("Kindly Select Any One Driver!")
	}
}


$scope.valsavedata  = function()
{
	var rdata = false;
	  for (var i = 0; i < $scope.TeamMaster.length; i++) {
		  if($scope.TeamMaster[i].DefaultDriverId!="")
		  {
			  rdata=true;
		  }
	  }
	  return rdata;
}




$scope.bulkupdate = function()
{
	var data = $scope.getupdatedata($scope.TeamMaster)
	//console.log(data);
	if(true)
	{
		if($scope.valsavedata())
		{
		US.UpdateIndividualPostalZoneMaster(data,$scope.user[0].CompanyCode).then(function (response) {alert(response.data[0].DisplayMessage);$scope.getteammaster();$scope.getdriver();});
		}
		else
		{
			alert("Kindly Select Any One Driver");
		}
	}
	else
	{
		alert("Kindly Change Any One Driver To Update.");
	}
}

$scope.checkDefaultDriver  = function()
{
	var rdata = false;
	  for (var i = 0; i < $scope.TeamMaster.length; i++) {
		  if($scope.TeamMaster[i].DefaultDriverId!="")
		  {
			  rdata=true;
		  }
	  }
	  return rdata;
}

$scope.getupdatedata = function(d)
{
	for (var i = 0; i < d.length; i++) {
		  if(d[i].NewDriverId == null)
		  {
			  d[i].NewDriverId = "";
			  d[i].DefaultDriverName = $scope.getdrivernamebyid(d[i].DefaultDriverId);
		  }
		  
		   d[i].CreatedDate = "";
			  d[i].DefaultDriverDate ="";
	  }
	  return d;
}

$scope.getsavedata = function(d)
{
	for (var i = 0; i < d.length; i++) {
		  if(d[i].DefaultDriverId!="")
		  {
			  d[i].DefaultDriverName = $scope.getdrivernamebyid(d[i].DefaultDriverId);
			  delete d[i].NewDriverId;
		  }
	  }
	  return d;
}

$scope.getdrivernamebyid = function(id){
	var rdata = null;
	  for (var i = 0; i < $scope.Drivers.length; i++) {
		  if($scope.Drivers[i].DriverId==id)
		  {
			  rdata=$scope.Drivers[i].DriverName;
		  }
	  }
	  return rdata;
}
$scope.closemenu = function()
{
	if($scope.bodyclass=='closed-sidebar')
		$scope.bodyclass="";
	else
		$scope.bodyclass="closed-sidebar";
}

$scope.compare = function() {
    $scope.result = false;
	  for (var i = 0; i < $scope.TeamMasterC.length; i++) {
		  
		  if($scope.TeamMasterC[i].NewDriverId !== $scope.TeamMaster[i].NewDriverId)
		  {
			  	$scope.result = true;
		  }
	  }
		  	
	return $scope.result;
  };

} ]);
