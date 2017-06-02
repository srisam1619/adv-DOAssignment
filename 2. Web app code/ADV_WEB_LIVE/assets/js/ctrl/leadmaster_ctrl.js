App.controller('leadmaster_ctrl', ['$scope', '$rootScope', '$http', '$window', '$cookies',

function ($scope, $rootScope, $http, $window, $cookies) {

    $scope.savelable = "Save";
    $scope.savebtn = false;
    $scope.data = { "LEADM": [{ "Code": "", "Name": "", "U_TelNo": "", "U_FaxNo": "", "U_MNo": "", "U_Email": ""}] };
    $rootScope.fulldata = '';
    $scope.update = false;
    $scope.findbtn = false;
    $scope.saveb = true;

    $scope.reset = function () {

        $scope.data.LEADM[0].Code = "";
        $scope.data.LEADM[0].Name = "";
        $scope.data.LEADM[0].U_TelNo = "";
        $scope.data.LEADM[0].U_FaxNo = "";
        $scope.data.LEADM[0].U_MNo = "";
        $scope.data.LEADM[0].U_Email = "";

        $scope.saveb = true;
        $scope.findbtn = false;
        $scope.update = false;



    }


    $scope.g_edit = function (i) {
        // alert(i);
        $scope.data.SQTOGEN[i].edit = true;
    }
    $scope.g_ok = function (i) {
        // alert(i);
        $scope.data.SQTOGEN[i].edit = false;
    }

    $scope.g_del = function (i) {
        //alert(i);
        $scope.data.SQTOGEN.splice(i, 1);
    }

    $scope.save = function () {
        $scope.savelable = "Loading..";
        $scope.savebtn = true;
        var config = {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded;charset=utf-8;'
            }
        }

        var parms = JSON.stringify($scope.data);
        $http.post("http://119.73.138.58:82/ICSB.asmx/LeadAdd", "value=" + parms, config)
   .then(
       function (response) {
           // success callback
           console.log(response.data);
           if (response.data.VALIDATE[0].Status == "True") {
               $scope.savelable = "Save";
               $scope.savebtn = false;
               $scope.reset();
               alert(response.data.VALIDATE[0].Msg);
           }
           else {
               alert(response.data.VALIDATE[0].Msg);
           }
       },
       function (response) {
           // failure callback

       }
    );
    }


    //findrecord

    $scope.find = function () {
        $scope.savelable = "Loading..";
        $scope.savebtn = true;
        //$scope.input = { "LEADM": [{ "Code": "001", "Name": "", "U_TelNo": "", "U_FaxNo": "", "U_MNo": "", "U_Email": ""}] }


        var config = {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded;charset=utf-8;'
            }
        }

        var parms = JSON.stringify($scope.data);
        $http.post("http://119.73.138.58:82/ICSB.asmx/LeadFindRecord", "value=" + parms, config)
   .then(
       function (response) {
           // success callback
           console.log(response.data);
           if (response.data.VALIDATE === undefined) {
               $scope.savelable = "Save";
               $scope.savebtn = false;
               $rootScope.fulldata = response.data;
               if ($rootScope.fulldata.LEADM.length > 1) {
                   //alert("more");
                   $("#myModal").modal()
               }
               else {
                   //alert("one");
                   $rootScope.findone(0);
               }
               //$scope.reset();
               //alert(response.data.VALIDATE[0].Msg);
           }
           else {
               alert(response.data.VALIDATE[0].Msg);
           }
       },
       function (response) {
           // failure callback

       }
    );
    }

    //get one record from multiple
    $rootScope.findone = function (i) {
        //alert(i);
        //console.log($rootScope.fulldata.LEADM[i].Code);
        $scope.data.LEADM[0].Code = $rootScope.fulldata.LEADM[i].Code;
        $scope.data.LEADM[0].Name = $rootScope.fulldata.LEADM[i].Name;
        $scope.data.LEADM[0].U_TelNo = $rootScope.fulldata.LEADM[i].U_TelNo;
        $scope.data.LEADM[0].U_FaxNo = $rootScope.fulldata.LEADM[i].U_FaxNo;
        $scope.data.LEADM[0].U_MNo = $rootScope.fulldata.LEADM[i].U_MNo;
        $scope.data.LEADM[0].U_Email = $rootScope.fulldata.LEADM[i].U_Email;

        $scope.saveb = false;
        $scope.findbtn = false;
        $scope.update = true;

    }





    //LeadLastRecord
    $scope.LeadLastRecord = function () {
        //$scope.savelable = "Loading..";
        $scope.savebtn = true;
        //$scope.input = { "LEADM": [{ "Code": "001", "Name": "", "U_TelNo": "", "U_FaxNo": "", "U_MNo": "", "U_Email": ""}] }


        var config = {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded;charset=utf-8;'
            }
        }

        var parms = JSON.stringify($scope.data);
        $http.post("http://119.73.138.58:82/ICSB.asmx/LeadLastRecord", "value=" + "", config)
   .then(
       function (response) {
           // success callback
           console.log(response.data);
           if (response.data.VALIDATE === undefined) {
               $scope.savelable = "Save";
               $scope.savebtn = false;
               $rootScope.fulldata = response.data;
               if ($rootScope.fulldata.LEADM.length > 1) {
                   //alert("more");
                   $("#myModal").modal()
               }
               else {
                   //alert("one");
                   $rootScope.findone(0);
               }
               //$scope.reset();
               //alert(response.data.VALIDATE[0].Msg);
           }
           else {
               alert(response.data.VALIDATE[0].Msg);
           }
       },
       function (response) {
           // failure callback

       }
    );
   }



   //LeadFirstRecord
   $scope.LeadFirstRecord = function () {
       //$scope.savelable = "Loading..";
       $scope.savebtn = true;
       //$scope.input = { "LEADM": [{ "Code": "001", "Name": "", "U_TelNo": "", "U_FaxNo": "", "U_MNo": "", "U_Email": ""}] }


       var config = {
           headers: {
               'Content-Type': 'application/x-www-form-urlencoded;charset=utf-8;'
           }
       }

       var parms = JSON.stringify($scope.data);
       $http.post("http://119.73.138.58:82/ICSB.asmx/LeadFirstRecord", "value=" + "", config)
   .then(
       function (response) {
           // success callback
           console.log(response.data);
           if (response.data.VALIDATE === undefined) {
               $scope.savelable = "Save";
               $scope.savebtn = false;
               $rootScope.fulldata = response.data;
               if ($rootScope.fulldata.LEADM.length > 1) {
                   //alert("more");
                   $("#myModal").modal()
               }
               else {
                   //alert("one");
                   $rootScope.findone(0);
               }
               //$scope.reset();
               //alert(response.data.VALIDATE[0].Msg);
           }
           else {
               alert(response.data.VALIDATE[0].Msg);
           }
       },
       function (response) {
           // failure callback

       }
    );
   }



   //LeadNextRecord
   $scope.LeadNextRecord = function () {
       //$scope.savelable = "Loading..";
       $scope.savebtn = true;
       //$scope.input = { "LEADM": [{ "Code": "001", "Name": "", "U_TelNo": "", "U_FaxNo": "", "U_MNo": "", "U_Email": ""}] }


       var config = {
           headers: {
               'Content-Type': 'application/x-www-form-urlencoded;charset=utf-8;'
           }
       }

       var parms = JSON.stringify({ "LEADM": [{ "Code": $scope.data.LEADM[0].Code}] });
       $http.post("http://119.73.138.58:82/ICSB.asmx/LeadNextRecord", "value=" + parms, config)
   .then(
       function (response) {
           // success callback
           console.log(response.data);
           if (response.data.VALIDATE === undefined) {
               $scope.savelable = "Save";
               $scope.savebtn = false;
               $rootScope.fulldata = response.data;
               if ($rootScope.fulldata.LEADM.length > 1) {
                   //alert("more");
                   $("#myModal").modal()
               }
               else {
                   //alert("one");
                   $rootScope.findone(0);
               }
               //$scope.reset();
               //alert(response.data.VALIDATE[0].Msg);
           }
           else {
               alert(response.data.VALIDATE[0].Msg);
           }
       },
       function (response) {
           // failure callback

       }
    );
   }


   //LeadPreviousRecord
   $scope.LeadPreviousRecord = function () {
       //$scope.savelable = "Loading..";
       $scope.savebtn = true;
       //$scope.input = { "LEADM": [{ "Code": "001", "Name": "", "U_TelNo": "", "U_FaxNo": "", "U_MNo": "", "U_Email": ""}] }


       var config = {
           headers: {
               'Content-Type': 'application/x-www-form-urlencoded;charset=utf-8;'
           }
       }

       var parms = JSON.stringify({ "LEADM": [{ "Code": $scope.data.LEADM[0].Code}] });
       $http.post("http://119.73.138.58:82/ICSB.asmx/LeadPreviousRecord", "value=" + parms, config)
   .then(
       function (response) {
           // success callback
           console.log(response.data);
           if (response.data.VALIDATE === undefined) {
               $scope.savelable = "Save";
               $scope.savebtn = false;
               $rootScope.fulldata = response.data;
               if ($rootScope.fulldata.LEADM.length > 1) {
                   //alert("more");
                   $("#myModal").modal()
               }
               else {
                   //alert("one");
                   $rootScope.findone(0);
               }
               //$scope.reset();
               //alert(response.data.VALIDATE[0].Msg);
           }
           else {
               alert(response.data.VALIDATE[0].Msg);
           }
       },
       function (response) {
           // failure callback

       }
    );
   }



   //updaterecord
   $scope.updaterecord = function () {
       $scope.savelable = "Loading..";
       $scope.savebtn = true;
       var config = {
           headers: {
               'Content-Type': 'application/x-www-form-urlencoded;charset=utf-8;'
           }
       }

       var parms = JSON.stringify($scope.data);
       $http.post("http://119.73.138.58:82/ICSB.asmx/LeadUpdate", "value=" + parms, config)
   .then(
       function (response) {
           // success callback
           console.log(response.data);
           if (response.data.VALIDATE[0].Status == "True") {
               $scope.savelable = "Save";
               $scope.savebtn = false;
               $scope.reset();
               alert(response.data.VALIDATE[0].Msg);
           }
           else {
               alert(response.data.VALIDATE[0].Msg);
           }
       },
       function (response) {
           // failure callback

       }
    );
   }


} ]);
