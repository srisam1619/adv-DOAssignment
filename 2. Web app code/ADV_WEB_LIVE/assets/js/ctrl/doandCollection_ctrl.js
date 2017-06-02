App.controller('doandCollection_ctrl', ['$scope', '$rootScope', '$http', '$window', '$cookies','util_SERVICE','$location',

function ($scope, $rootScope, $http, $window, $cookies,US,$location) {
	US.islogin();
    $scope.savelable = "Save";
	$scope.updatebtn = false;
	$scope.printbtn = false;
    $scope.user = angular.fromJson($cookies.get('UserData'));
	
	
	$scope.custname = "";
	$scope.docnumber=""
	$scope.driver1="";
	$scope.statusss="New";
	$scope.type="";
	
	$scope.d = new Date();
    $scope.n = $scope.d.getHours();
	
	if($scope.n<9 || $scope.n>13)
	{
		$scope.ampm = "PM";
	}
	else
	{
		$scope.ampm = "AM";
	}
	
	$scope.date = ""

	var today = new Date();
	var dd = today.getDate();
	var mm = today.getMonth()+1; //January is 0!
	
	var yyyy = today.getFullYear();
	if(dd<10){
		dd='0'+dd
	} 
	if(mm<10){
		mm='0'+mm
	} 
	

	$scope.Fdate = yyyy+'-'+mm+'-'+dd;
	$scope.Tdate =  yyyy+'-'+mm+'-'+dd;
	
	$scope.opens = function($event, dt) {
    $event.preventDefault();
    $event.stopPropagation();

    dt.opened = true;
  };



//change color using status 
$scope.whatClassIsIt = function(type)
{

	
		if(type=="Delivery")
			return "blue";
		if(type=="Collection")
			return "Yellow";
		if(type=="Define")
			return "LightRed";
		if(type=="Internal")
			return "white";
		
}
$scope.datechange = function(a)
{
	alert(a);
}
$scope.searchmaster = function()
{
	
	
	if($scope.Fdate == undefined || $scope.Tdate == undefined)
	{
		alert("Kindly Select From & To Date!");
	}
	else
	{
		US.DOA_SearchOtherDOandCN($scope.Fdate,$scope.Tdate,$scope.user[0].CompanyCode,$scope.custname,$scope.docnumber,$scope.driver1,$scope.statusss,$scope.type).then(function (response) 
		{
			$scope.masterDrivers = response.data;
			
			console.log($scope.masterDrivers);
			
			
		});
		
	}
	
}

$scope.copytoall = function(num)
{
	var i;
	if($scope.masterDrivers[num].ItemsArray.length>1)
	{
		for(i=0;i<$scope.masterDrivers[num].ItemsArray.length;i++)
		{
			$scope.masterDrivers[num].ItemsArray[i].DeliveryStatus = $scope.masterDrivers[num].ItemsArray[0].DeliveryStatus;
			$scope.masterDrivers[num].ItemsArray[i].LineRemarks = $scope.masterDrivers[num].ItemsArray[0].LineRemarks;
		}
	}
	
	
}


$scope.changedriver = function()
{
	if($scope.driver2 == undefined || $scope.driver2 == "")
	{
		alert("Kindly Select New Driver To update!");
	}
	else
	{
		var msg = "Do you wish to update all DO ("+$scope.DOSdata.length+") below to the new Driver Name-"+$scope.driver2+" ?";
		var r = confirm(msg);
		if (r == true) {
			var data = $scope.getreplacedata($scope.DOSdata);
				US.DOA_ReplaceDODrivers(data,$scope.user[0].CompanyCode).then(function (response) 
				{
					$scope.result = response.data;
					if($scope.result[0].Result=="SUCCESS")
					{
						alert($scope.result[0].DisplayMessage);
						$scope.searchmaster();
					}
					else
					{
						alert($scope.result[0].DisplayMessage);
					}
					
				});
		} else {
			
		}


		
	}
}


$scope.getreplacedata = function(d)
{
	for (var i = 0; i < d.length; i++) {

			  d[i].Driver = angular.copy($scope.driver2);
			  delete d[i].ItemsArray;
		  
	  }
	  return d;
}

	
	
$scope.searchmaster();

    US.DO_GetDrivers($scope.user[0].CompanyCode).then(function (response) 
	{
		$scope.Drivers =  $scope.getdervernameonly(response.data);
	});
	
	
	
	
$scope.getdervernameonly = function(data)
	{
		var rdata = [];
		 for (var i = 0; i < data.length; i++) {
		  rdata.push(angular.copy(data[i].Driver));
	  }
		return rdata;
	}
	
	
	$scope.updateDO = function()
	{
		$scope.updatebtn = true;
		//$scope.data = $scope.getsavedata($scope.DOSdata);
		
		US.Doa_updateotherdoandcn($scope.masterDrivers,$scope.user[0].CompanyCode).then(function (response) 
				{
					$scope.updatebtn = false;
					$scope.result = response.data;
					if($scope.result[0].Result=="SUCCESS")
					{
						alert($scope.result[0].DisplayMessage);
						$scope.searchmaster();
						
					}
					else
					{
						alert($scope.result[0].DisplayMessage);
						
					}
					
				});
	}
	
	
	 $scope.getteammaster = function()
	 {
		 if(angular.element('#mydate').val()!="")
		 {
	 US.SaveDriverListByDay($scope.user[0].CompanyCode,angular.element('#mydate').val()).then(function (response) { $scope.TeamMaster = $scope.addnewdriver(response.data);
		  if($scope.checkDefaultDriver())
		  {
			  $scope.updatebtn = true;
			  $scope.savebtn = false;
			  $scope.bulkupdatebtn = true;
		  }
		  else
		  {
			  $scope.updatebtn = false;
			  $scope.savebtn = false;
			  $scope.bulkupdatebtn = true;
		  }});
		 }
		 else
		 {
			 alert("Kindly Select Date.");
		 }
	 }
	 
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
	else if(angular.element('#mydate').val()=="")
	{
		 alert("Kindly Select Date.");
	}
	else
	{
	US.UpdateBulkDriverListByDay($scope.driver1,$scope.driver2,$scope.user[0].CompanyCode,angular.element('#mydate').val()).then(function (response) {console.log(response);alert(response.data[0].DisplayMessage);$scope.getteammaster();});
	}
}

$scope.save = function()
{
	var data = $scope.getsavedata($scope.TeamMaster)
	//console.log(data);
	US.SavePostalZoneMaster(data,$scope.user[0].CompanyCode).then(function (response) {alert(response.data[0].DisplayMessage);$scope.getteammaster();});
}

$scope.bulkupdate = function()
{
	var data = $scope.getupdatedata($scope.TeamMaster)
	//console.log(data);
	if($scope.checkDefaultDriver())
	{
	US.UpdateIndividualDriverListByDay(data,$scope.user[0].CompanyCode).then(function (response) {alert(response.data[0].DisplayMessage);$scope.getteammaster();});
	}
	else
	{
		alert("Kindly Select Any One Driver");
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
			  d[i].Email = $scope.getdriverEmail(d[i].Driver);
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


 $scope.getdriverEmail = function(name){
	var rdata = null;
	  for (var i = 0; i < $scope.NewDrivers.length; i++) {
		  if($scope.NewDrivers[i].Driver==name)
		  {
			  rdata=$scope.NewDrivers[i].Type;
		  }
	  }
	  console.log(name);
	  return rdata;
}
 
 
 $scope.printdo = function(number)
 {
	 //alert(number);
	 US.CreatePDF(number,$scope.user[0].CompanyCode).then(function (response) 
				{
					console.log(response);
					if(response.data[0].Result=="Success")
					{
						var win = window.open(US.Burl+"/Reports/print/report_print.html?url="+US.Burl+response.data[0].DisplayMessage, '_blank');
 						win.focus();
					}
					else
					{
						alert(response.data[0].DisplayMessage);
					}
				});
 }
 
 $scope.scroolup = function()
 {
	 window.scrollTo(0, 0);
 }
 
$scope.closemenu = function()
{
	if($scope.bodyclass=='closed-sidebar')
		$scope.bodyclass="";
	else
		$scope.bodyclass="closed-sidebar";
}
$scope.closemenu();
} ]);


//fixed search bar 
$(window).bind('scroll', function () {
	var up = $('.upfixed');							   
    var menu = $('.searchbar');
    if ($(window).scrollTop() > 200) {
        menu.addClass('fixed');
		menu.removeClass('row');
		up.addClass('upfixedShow');
		up.removeClass('upfixedHide');
    } else {
        menu.removeClass('fixed');
		menu.addClass('row');
		up.addClass('upfixedHide');
		up.removeClass('upfixedShow');
    }
});

