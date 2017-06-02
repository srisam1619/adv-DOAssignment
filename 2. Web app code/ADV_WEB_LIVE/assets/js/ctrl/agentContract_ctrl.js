App.controller('postal_ctrl', ['$scope', '$rootScope', '$http', '$window', '$cookies',

function ($scope, $rootScope, $http, $window, $cookies) {
    $scope.savelable = "Save";
    $scope.savebtn = false;
    $scope.data = {
    "ACON": [
        {
            "U_Conno": "1103",
            "U_Status": "OPEN",
            "U_Uname": "Bathra",
            "U_Cdate": "20-03-2016",
            "U_Agentcode": "0005",
            "U_Agentname": "RAM",
            "U_CPeriod1": "20-03-2016",
            "U_CPeriod2": "20-03-2018",
            "U_Pcode": "RRD1",
            "U_AddrN": "ajithnagar",
            "U_Addr1": "annanagar",
            "U_Addr2": "annanagar",
            "U_Addr3": "annanagar",
            "U_Addr4": "annanagar",
            "U_Addr5": "annanagar",
            "U_Addr6": "annanagar",
            "U_TelNo": "5210422",
            "U_FaxNo": "52120422",
            "U_Mno": "9629476950",
            "U_Email": "madeswaran1986@gmail.com",
            "U_Remarks": "madeswaran1986@gmail.com"
        }
    ],
    "ACONGEN": [
        {
            "U_Ccode": "002",
            "U_Cname": "Kannan",
            "U_Stype": "General",
            "U_Conti": "56",
            "U_Country": "India",
            "U_City": "namakkal",
            "U_Currency": "INR",
            "U_EQGroup": "medical",
            "U_Rate": "56",
            "U_UOM": "UNIT",
            "U_GST": "GST 2 %",
            "U_Remarks": "Testing data values"
        },
        {
            "U_Ccode": "002",
            "U_Cname": "Kannan",
            "U_Stype": "Sub",
            "U_Conti": "20",
            "U_Country": "Signapore",
            "U_City": "namakkal",
            "U_Currency": "INR",
            "U_EQGroup": "medical",
            "U_Rate": "56",
            "U_UOM": "UNIT",
            "U_GST": "GST 4 %",
            "U_Remarks": "Testing data values2"
        },
        {
            "U_Ccode": "002",
            "U_Cname": "Kannan",
            "U_Stype": "Private",
            "U_Conti": "26",
            "U_Country": "China",
            "U_City": "namakkal",
            "U_Currency": "INR",
            "U_EQGroup": "medical",
            "U_Rate": "56",
            "U_UOM": "UNIT",
            "U_GST": "GST 5 %",
            "U_Remarks": "Testing data values3"
        }
    ],
    "ACONADD": [
        {
            "U_Ccode": "002",
            "U_Cname": "Kannan",
            "U_Ctype": "General",
            "U_Continent": "56",
            "U_Country": "India",
            "U_City": "namakkal",
            "U_Currency": "INR",
            "U_EQGroup": "medical",
            "U_Rate": "56",
            "U_UOM": "UNIT",
            "U_GST": "GST 2 %",
            "U_Remarks": "Testing data values"
        },
        {
            "U_Ccode": "002",
            "U_Cname": "Kannan",
            "U_Ctype": "Sub",
            "U_Continent": "56",
            "U_Country": "India",
            "U_City": "namakkal",
            "U_Currency": "INR",
            "U_EQGroup": "medical",
            "U_Rate": "56",
            "U_UOM": "UNIT",
            "U_GST": "GST 2 %",
            "U_Remarks": "Testing data values"
        },
        {
            "U_Ccode": "002",
            "U_Cname": "Kannan",
            "U_Ctype": "Private",
            "U_Continent": "56",
            "U_Country": "India",
            "U_City": "namakkal",
            "U_Currency": "INR",
            "U_EQGroup": "medical",
            "U_Rate": "56",
            "U_UOM": "UNIT",
            "U_GST": "GST 2 %",
            "U_Remarks": "Testing data values"
        }
    ]
}

    $scope.g_add = function () {

        $scope.gen = {
            "U_Ccode": $scope.g_Ccode,
            "U_Cname": $scope.g_Cname,
            "U_Stype": $scope.g_Stype,
            "U_Conti": $scope.g_Continent,
            "U_Country": $scope.g_Country,
            "U_City": $scope.g_city,
            "U_Currency": $scope.g_curency,
            "U_EQGroup": $scope.g_egroup,
            "U_Rate": $scope.g_rate,
            "U_UOM": $scope.g_uom,
            "U_GST": $scope.g_gst,
            "U_Remarks": $scope.g_remark,
        };
        $scope.data.ACONGEN.push($scope.gen);
        $scope.g_reset();
    }

    $scope.g_reset = function () {
            $scope.g_Ccode = "",
            $scope.g_Cname = "",
            $scope.g_Stype = "";
            $scope.g_Continent= "";
            $scope.g_Country= "";
            $scope.g_city= "";
            $scope.g_curency= "";
            $scope.g_egroup= "";
            $scope.g_rate= "";
            $scope.g_uom= "";
            $scope.g_gst= "";
            $scope.g_remark= "";
        
        
    }


    $scope.g_edit = function(i)
    {
       // alert(i);
        $scope.data.ACONGEN[i].edit = true;
    }
    $scope.g_ok = function(i)
    {
       // alert(i);
        $scope.data.ACONGEN[i].edit = false;
    }

    $scope.g_del = function(i)
    {
        //alert(i);
        $scope.data.ACONGEN.splice(i, 1); 
    }
     $scope.g_copy = function(i)
    {  
          
            $scope.g_Ccode = $scope.data.ACONGEN[i].U_Ccode;
            $scope.g_Cname= $scope.data.ACONGEN[i].U_Cname;
            $scope.g_Stype = $scope.data.ACONGEN[i].U_Stype;
            $scope.g_Continent= $scope.data.ACONGEN[i].U_Conti;
            $scope.g_Country= $scope.data.ACONGEN[i].U_Country;
            $scope.g_city= $scope.data.ACONGEN[i].U_City;
            $scope.g_curency= $scope.data.ACONGEN[i].U_Currency;
            $scope.g_egroup= $scope.data.ACONGEN[i].U_EQGroup;
            $scope.g_rate= $scope.data.ACONGEN[i].U_rate;
            $scope.g_uom= $scope.data.ACONGEN[i].U_UOM;
            $scope.g_gst= $scope.data.ACONGEN[i].U_GST;
            $scope.g_remark= $scope.data.ACONGEN[i].U_Remarks;
    }




    //add-on functions
    
    $scope.a_add = function () {

        $scope.add = {
            "U_Ctype": $scope.a_Ctype,
            "U_Continent": $scope.a_Continent,
            "U_Country": $scope.a_Country,
            "U_City": $scope.a_city,
            "U_Currency": $scope.a_curency,
            "U_EQGroup": $scope.a_egroup,
            "U_Rate": $scope.a_rate,
            "U_UOM": $scope.a_uom,
            "U_GST": $scope.a_gst,
            "U_Remarks": $scope.a_remark,
        };
        $scope.data.ACONADD.push($scope.add);
        $scope.a_reset();
    }

    $scope.a_reset = function () {

            $scope.a_Ctype = "";
            $scope.a_Continent= "";
            $scope.a_Country= "";
            $scope.a_city= "";
            $scope.a_curency= "";
            $scope.a_egroup= "";
            $scope.a_rate= "";
            $scope.a_uom= "";
            $scope.a_gst= "";
            $scope.a_remark= "";
        
        
    }


    $scope.a_edit = function(i)
    {
       // alert(i);
        $scope.data.ACONADD[i].edit = true;
    }
    $scope.a_ok = function(i)
    {
       // alert(i);
        $scope.data.ACONADD[i].edit = false;
    }

    $scope.a_del = function(i)
    {
        //alert(i);
        $scope.data.ACONADD.splice(i, 1); 
    }
     $scope.a_copy = function(i)
    { 
            $scope.a_Ctype = $scope.data.ACONADD[i].U_Ctype;
            $scope.a_Continent= $scope.data.ACONADD[i].U_Continent;
            $scope.a_Country= $scope.data.ACONADD[i].U_Country;
            $scope.a_city= $scope.data.ACONADD[i].U_City;
            $scope.a_curency= $scope.data.ACONADD[i].U_Currency;
            $scope.a_egroup= $scope.data.ACONADD[i].U_EQGroup;
            $scope.a_rate= $scope.data.ACONADD[i].U_rate;
            $scope.a_uom= $scope.data.ACONADD[i].U_UOM;
            $scope.a_gst= $scope.data.ACONADD[i].U_GST;
            $scope.a_remark= $scope.data.ACONADD[i].U_Remarks;
    }
    

    //resetall field
    $scope.resetall = function()
    {
             $scope.data = {
            "ACON": [{
                "U_Qno": "",
                "U_Status": "",
                "U_Uname": "",
                "U_Cdate": "",
                "U_Ccode": "",
                "U_Cname": "",
                "U_CPeriod1": "",
                "U_CPeriod2": "",
                "U_Pcode": "",
                "U_AddrN": "",
                "U_Addr1": "",
                "U_Addr2": "",
                "U_Addr3": "",
                "U_Addr4": "",
                "U_Addr5": "",
                "U_Addr6": "",
                "U_TelNo": "",
                "U_FaxNo": "",
                "U_Mno": "",
                "U_Email": "",
                "U_Remarks": ""
            }],
            "ACONGEN": [],
            "ACONADD": []
        };
    }

     $scope.resetall();
    //save service call
    $scope.save = function()
    {
        $scope.savelable = "Loading..";
        $scope.savebtn = true;
        var config = {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded;charset=utf-8;'
            }
        }

        var parms = encodeURIComponent(JSON.stringify($scope.data));
        $http.post("http://119.73.138.58:82/ICSB.asmx/AgentContractAdd", "value=" + parms, config)
   .then(
       function (response) {
           // success callback
           console.log(response.data);
           if (response.data.VALIDATE[0].Status == "Ture") {
              $scope.savelable = "Save";
              $scope.savebtn = false;
              $scope.resetall();
              alert(response.data.VALIDATE[0].Msg);
           }
            else
            {
               alert(response.data.VALIDATE[0].Msg);
                $scope.savelable = "Save";
                $scope.savebtn = false;
               }
       },
       function (response) {
           // failure callback

       }
    );
    }

} ]);
