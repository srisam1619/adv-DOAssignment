using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Reflection;
using ADV_BLL_V001;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using CrystalDecisions.Shared;

namespace ADV_WebService_V001
{
    /// <summary>
    /// Summary description for DOService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class DOService : System.Web.Services.WebService
    {
        #region Objects

        clsLog oLog = new clsLog();
        clsDataAccess oDataAccess = new clsDataAccess();
        public string sErrDesc = string.Empty;
        List<result> lstResult = new List<result>();
        JavaScriptSerializer js = new JavaScriptSerializer();
        SAPbobsCOM.Company oDICompany;
        SendDOEmail oSendDOEmail = new SendDOEmail();
        public static string postalMasterDBName = System.Configuration.ConfigurationManager.AppSettings["postalMasterDBName"].ToString();

        #endregion

        #region Web Methods

        #region Login

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetCompanyList()
        {
            string sFuncName = string.Empty;
            try
            {
                sFuncName = "GetCompanyList";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                oLog.WriteToDebugLogFile("Before calling the Get_CompanyList() ", sFuncName);
                DataSet ds = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After calling the Get_CompanyList() ", sFuncName);
                if (ds != null && ds.Tables.Count > 0)
                {
                    List<Company> lstCompany = new List<Company>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Company _company = new Company();
                        _company.U_DBName = r["U_DBName"].ToString();
                        _company.U_CompName = r["U_CompName"].ToString();
                        _company.U_SAPUserName = r["U_SAPUserName"].ToString();
                        _company.U_SAPPassword = r["U_SAPPassword"].ToString();
                        _company.U_DBUserName = r["U_DBUserName"].ToString();
                        _company.U_DBPassword = r["U_DBPassword"].ToString();
                        _company.U_ConnString = r["U_ConnString"].ToString();
                        _company.U_Server = r["U_Server"].ToString();
                        _company.U_LicenseServer = r["U_LicenseServer"].ToString();
                        lstCompany.Add(_company);
                    }
                    oLog.WriteToDebugLogFile("Before Serializing the Company List ", sFuncName);
                    Context.Response.Output.Write(js.Serialize(lstCompany));
                    oLog.WriteToDebugLogFile("After Serializing the Company List , the Serialized data is ' " + js.Serialize(lstCompany) + " '", sFuncName);
                }
                else
                {
                    List<Company> lstCompany = new List<Company>();
                    Context.Response.Output.Write(js.Serialize(lstCompany));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void LoginValidation(string sJsonInput)
        {
            string sFuncName = string.Empty;
            try
            {
                sFuncName = "LoginValidation()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                string sUserName = string.Empty;
                string sPassword = string.Empty;
                string sCompany = string.Empty;

                sJsonInput = "[" + sJsonInput + "]";
                //Split JSON to Individual String
                oLog.WriteToDebugLogFile("Getting the Json Input from Mobile  '" + sJsonInput + "'", sFuncName);
                oLog.WriteToDebugLogFile("Before Deserialize the Json Input ", sFuncName);
                List<Json_UserInfo> lstDeserialize = js.Deserialize<List<Json_UserInfo>>(sJsonInput);
                oLog.WriteToDebugLogFile("After Deserialize the Json Input ", sFuncName);
                if (lstDeserialize.Count > 0)
                {
                    Json_UserInfo objUserInfo = lstDeserialize[0];
                    sUserName = objUserInfo.sUserName;
                    sPassword = objUserInfo.sPassword;
                    sCompany = objUserInfo.sCompany;
                }

                DataSet oDTCompanyList = new DataSet();
                oLog.WriteToDebugLogFile("Before calling the Get_CompanyList() ", sFuncName);
                oDTCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After calling the Get_CompanyList() ", sFuncName);
                Session["ODTCompanyList"] = oDTCompanyList;
                oLog.WriteToDebugLogFile("Before calling the Method LoginValidation() ", sFuncName);
                DataSet ds = oDataAccess.LoginValidation(oDTCompanyList, sUserName, sPassword, sCompany);
                oLog.WriteToDebugLogFile("After calling the Method LoginValidation() ", sFuncName);
                if (ds != null && ds.Tables.Count > 0)
                {
                    List<UserInfo> lstUserInfo = new List<UserInfo>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        UserInfo _userInfo = new UserInfo();
                        _userInfo.UserId = r["UserId"].ToString();
                        _userInfo.UserName = r["UserName"].ToString();
                        _userInfo.CompanyCode = r["CompanyCode"].ToString();
                        _userInfo.Message = r["Message"].ToString();
                        lstUserInfo.Add(_userInfo);
                    }
                    oLog.WriteToDebugLogFile("Before Serializing the UserInformation ", sFuncName);
                    Context.Response.Output.Write(js.Serialize(lstUserInfo));
                    oLog.WriteToDebugLogFile("After Serializing the UserInformation , the Serialized data is ' " + js.Serialize(lstUserInfo) + " '", sFuncName);
                }
                else
                {
                    List<UserInfo> lstUserInfo = new List<UserInfo>();
                    UserInfo objUserInfo = new UserInfo();
                    objUserInfo.UserId = string.Empty;
                    objUserInfo.UserName = string.Empty;
                    objUserInfo.CompanyCode = sCompany;
                    objUserInfo.Message = "UserName/ Password is Incorrect";
                    lstUserInfo.Add(objUserInfo);

                    Context.Response.Output.Write(js.Serialize(lstUserInfo));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        #endregion

        #region Postal Zone Master

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetDrivers(string sJsonInput)
        {
            string sFuncName = string.Empty;
            try
            {
                sFuncName = "GetDrivers()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                string sCompany = string.Empty;

                sJsonInput = "[" + sJsonInput + "]";
                //Split JSON to Individual String
                oLog.WriteToDebugLogFile("Getting the Json Input from Mobile  '" + sJsonInput + "'", sFuncName);
                oLog.WriteToDebugLogFile("Before Deserialize the Json Input ", sFuncName);
                List<Json_TeamMaster> lstDeserialize = js.Deserialize<List<Json_TeamMaster>>(sJsonInput);
                oLog.WriteToDebugLogFile("After Deserialize the Json Input ", sFuncName);
                if (lstDeserialize.Count > 0)
                {
                    Json_TeamMaster objDrivers = lstDeserialize[0];
                    sCompany = objDrivers.sCompany;
                }

                oLog.WriteToDebugLogFile("Before Retrieving the Company list from session ", sFuncName);

                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list from session ", sFuncName);
                oLog.WriteToDebugLogFile("Before Calling the method Get_Drivers() ", sFuncName);
                DataSet ds = oDataAccess.Get_Drivers(dsCompanyList, sCompany);
                oLog.WriteToDebugLogFile("After Calling the method Get_Drivers() ", sFuncName);
                if (ds != null && ds.Tables.Count > 0)
                {
                    List<Drivers> lstDrivers = new List<Drivers>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Drivers _Drivers = new Drivers();
                        _Drivers.DriverId = r["DriverId"].ToString();
                        _Drivers.DriverName = r["DriverName"].ToString();
                        lstDrivers.Add(_Drivers);
                    }
                    oLog.WriteToDebugLogFile("Before Serializing the Drivers list ", sFuncName);
                    Context.Response.Output.Write(js.Serialize(lstDrivers));
                    oLog.WriteToDebugLogFile("After Serializing the Drivers list, the serialized data is ' " + js.Serialize(lstDrivers) + " '", sFuncName);

                }
                else
                {
                    List<Drivers> lstDrivers = new List<Drivers>();
                    Context.Response.Output.Write(js.Serialize(lstDrivers));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetTeamMaster(string sJsonInput)
        {
            string sFuncName = string.Empty;
            try
            {
                sFuncName = "GetTeamMaster()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                string sCompany = string.Empty;
                string sDriverDate = string.Empty;

                sJsonInput = "[" + sJsonInput + "]";
                //Split JSON to Individual String
                oLog.WriteToDebugLogFile("Getting the Json Input from Mobile  '" + sJsonInput + "'", sFuncName);
                oLog.WriteToDebugLogFile("Before Deserialize the Json Input ", sFuncName);
                List<Json_TeamMaster> lstDeserialize = js.Deserialize<List<Json_TeamMaster>>(sJsonInput);
                oLog.WriteToDebugLogFile("After Deserialize the Json Input ", sFuncName);
                if (lstDeserialize.Count > 0)
                {
                    Json_TeamMaster objDrivers = lstDeserialize[0];
                    sCompany = objDrivers.sCompany;
                    sDriverDate = objDrivers.sDriverDate;
                }

                oLog.WriteToDebugLogFile("Before Retrieving the Company list from session ", sFuncName);

                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list from session ", sFuncName);
                oLog.WriteToDebugLogFile("Before Calling the method Get_TeamMaster() ", sFuncName);
                DataSet ds = oDataAccess.Get_TeamMaster(dsCompanyList, sCompany, sDriverDate);
                oLog.WriteToDebugLogFile("After Calling the method Get_TeamMaster() ", sFuncName);
                if (ds != null && ds.Tables.Count > 0)
                {
                    List<TeamMaster> lstTeamMaster = new List<TeamMaster>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        TeamMaster _TeamMaster = new TeamMaster();
                        _TeamMaster.Id = r["Id"].ToString();
                        _TeamMaster.TeamId = r["TeamId"].ToString();
                        _TeamMaster.TeamName = r["TeamName"].ToString();
                        _TeamMaster.Description = r["Description"].ToString();
                        _TeamMaster.DefaultDriverId = r["DefaultDriverId"].ToString();
                        _TeamMaster.DefaultDriverName = r["DefaultDriverName"].ToString();
                        _TeamMaster.DefaultDriverDate = string.IsNullOrEmpty(r["DefaultDriverDate"].ToString()) ? (DateTime?)null : DateTime.Parse(r["DefaultDriverDate"].ToString());
                        _TeamMaster.CreatedDate = string.IsNullOrEmpty(r["CreatedDate"].ToString()) ? (DateTime?)null : DateTime.Parse(r["CreatedDate"].ToString());
                        _TeamMaster.IsMaster = r["IsMaster"].ToString();
                        lstTeamMaster.Add(_TeamMaster);
                    }
                    oLog.WriteToDebugLogFile("Before Serializing the TeamMaster list ", sFuncName);
                    Context.Response.Output.Write(js.Serialize(lstTeamMaster));
                    oLog.WriteToDebugLogFile("After Serializing the TeamMaster list, the serialized data is ' " + js.Serialize(lstTeamMaster) + " '", sFuncName);

                }
                else
                {
                    List<TeamMaster> lstTeamMaster = new List<TeamMaster>();
                    Context.Response.Output.Write(js.Serialize(lstTeamMaster));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void SavePostalZoneMaster(string sJsonInput, string sCompany)
        {
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            try
            {
                sFuncName = "SavePostalZoneMaster()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                sJsonInput = "[" + sJsonInput + "]";
                //Convert JSON to Array
                oLog.WriteToDebugLogFile("Getting the Json Input from Mobile  '" + sJsonInput + "'", sFuncName);
                oLog.WriteToDebugLogFile("Before Converting the Json Input to datatable", sFuncName);
                DataTable dtResult = JsonStringToDataTable(sJsonInput);
                oLog.WriteToDebugLogFile("After Converting the Json Input to datatable", sFuncName);

                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list", sFuncName);

                oDTView = dsCompanyList.Tables[0].DefaultView;

                oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                oLog.WriteToDebugLogFile("Before Connecting to SQL", sFuncName);
                if (oDTView != null && oDTView.Count > 0)
                {
                    using (SqlConnection connection = new SqlConnection(oDTView[0]["U_ConnString"].ToString()))
                    {
                        SqlBulkCopy bulkCopy = new SqlBulkCopy
                        (
                        connection,
                        SqlBulkCopyOptions.TableLock |
                        SqlBulkCopyOptions.FireTriggers |
                        SqlBulkCopyOptions.UseInternalTransaction,
                        null
                        );

                        bulkCopy.DestinationTableName = postalMasterDBName.ToString();
                        connection.Open();
                        oLog.WriteToDebugLogFile("Before Writing to SQL", sFuncName);
                        // write the data in the "dataTable"
                        bulkCopy.WriteToServer(dtResult);
                        oLog.WriteToDebugLogFile("After Writing to SQL", sFuncName);
                        connection.Close();
                        oLog.WriteToDebugLogFile("Completed With SUCCESS  ", sFuncName);
                        result objResult = new result();
                        objResult.Result = "Success";
                        objResult.DisplayMessage = "Postal Zone Default Master Added Successfully";
                        lstResult.Add(objResult);
                        Context.Response.Output.Write(js.Serialize(lstResult));
                    }
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void UpdateBulkPostalZoneMaster(string sOldDriverId, string sNewDriverId, string sCompany)
        {
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            try
            {
                sFuncName = "UpdateBulkPostalZoneMaster()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                oLog.WriteToDebugLogFile("Before Retrieving the Company list", sFuncName);

                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list", sFuncName);
                oLog.WriteToDebugLogFile("Before Calling the method UpdateBulkPostalZoneMaster() ", sFuncName);
                int returnResult = oDataAccess.UpdateBulkPostalZoneMaster(dsCompanyList, sCompany, sOldDriverId, sNewDriverId);
                oLog.WriteToDebugLogFile("After Calling the method UpdateBulkPostalZoneMaster() ", sFuncName);
                if (returnResult != 0)
                {
                    oLog.WriteToDebugLogFile("Completed With SUCCESS  ", sFuncName);
                    result objResult = new result();
                    objResult.Result = "SUCCESS";
                    objResult.DisplayMessage = "Postal Zone Default Master Updated Successfully";
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
                else
                {
                    oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                    result objResult = new result();
                    objResult.Result = "Error";
                    objResult.DisplayMessage = "Postal Zone Default Master Update Failed";
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void UpdateIndividualPostalZoneMaster(string sJsonInput, string sCompany)
        {
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            DataTable dtResult = new DataTable();
            try
            {
                sFuncName = "UpdateIndividualPostalZoneMaster()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                //sJsonInput = "[" + sJsonInput + "]";
                //Convert JSON to Array
                oLog.WriteToDebugLogFile("Getting the Json Input from Mobile  '" + sJsonInput + "'", sFuncName);

                oLog.WriteToDebugLogFile("Before Deserialize the Json Input ", sFuncName);
                List<UpdatePostalMaster> PostalMaster = js.Deserialize<List<UpdatePostalMaster>>(sJsonInput);
                oLog.WriteToDebugLogFile("After Deserialize the Json Input ", sFuncName);
                dtResult = ToDataTable<UpdatePostalMaster>(PostalMaster);

                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list", sFuncName);

                oLog.WriteToDebugLogFile("Before Calling the method UpdateIndividualPostalZoneMaster() ", sFuncName);
                int returnResult = oDataAccess.UpdateIndividualPostalZoneMaster(dsCompanyList, sCompany, dtResult);
                oLog.WriteToDebugLogFile("After Calling the method UpdateIndividualPostalZoneMaster() ", sFuncName);
                returnResult = returnResult + 1;
                if (returnResult != 0)
                {
                    oLog.WriteToDebugLogFile("Completed With SUCCESS  ", sFuncName);
                    result objResult = new result();
                    objResult.Result = "SUCCESS";
                    objResult.DisplayMessage = "Postal Zone Default Master Updated Successfully";
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
                else
                {
                    oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                    result objResult = new result();
                    objResult.Result = "Error";
                    objResult.DisplayMessage = "Postal Zone Default Master Update Failed";
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        #endregion

        #region Driver list by day

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void SaveDriverListByDay(string sDriverDate, string sCompany)
        {
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            try
            {
                sFuncName = "SaveDriverListByDay()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                oLog.WriteToDebugLogFile("Getting the Input from Mobile  DriverDate : '" + sDriverDate + "' and Company : '" + sCompany + "'", sFuncName);
                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list", sFuncName);

                oLog.WriteToDebugLogFile("Before Calling the method Get_DriverListbyDay() ", sFuncName);
                DataSet ds = oDataAccess.Get_DriverListbyDay(dsCompanyList, sCompany, sDriverDate);
                oLog.WriteToDebugLogFile("After Calling the method Get_DriverListbyDay() ", sFuncName);

                if (ds != null && ds.Tables.Count != 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        object sumObject;
                        sumObject = ds.Tables[0].Compute("Sum(Id)", "");

                        // Check whether the Record exists for the selected date , if not , Insertion happens
                        if (sumObject.ToString() == "0")
                        {
                            oDTView = dsCompanyList.Tables[0].DefaultView;

                            oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                            oLog.WriteToDebugLogFile("Before Connecting to SQL", sFuncName);
                            if (oDTView != null && oDTView.Count > 0)
                            {
                                using (SqlConnection connection = new SqlConnection(oDTView[0]["U_ConnString"].ToString()))
                                {
                                    SqlBulkCopy bulkCopy = new SqlBulkCopy
                                    (
                                    connection,
                                    SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers |
                                    SqlBulkCopyOptions.UseInternalTransaction,
                                    null
                                    );

                                    bulkCopy.DestinationTableName = postalMasterDBName.ToString();
                                    connection.Open();
                                    oLog.WriteToDebugLogFile("Before Writing to SQL", sFuncName);
                                    // write the data in the "dataTable"
                                    bulkCopy.WriteToServer(ds.Tables[0]);
                                    oLog.WriteToDebugLogFile("After Writing to SQL", sFuncName);
                                    connection.Close();
                                    oLog.WriteToDebugLogFile("Completed With SUCCESS  ", sFuncName);

                                    oLog.WriteToDebugLogFile("Before Fetching the inserted Record", sFuncName);
                                    DataSet dsAfterSuccess = oDataAccess.Get_DriverListbyDay(dsCompanyList, sCompany, sDriverDate);
                                    oLog.WriteToDebugLogFile("After Fetching the inserted Record", sFuncName);

                                    List<TeamMaster> lstTeamMaster = new List<TeamMaster>();
                                    foreach (DataRow r in dsAfterSuccess.Tables[0].Rows)
                                    {
                                        TeamMaster _TeamMaster = new TeamMaster();
                                        _TeamMaster.Id = r["Id"].ToString();
                                        _TeamMaster.TeamId = r["TeamId"].ToString();
                                        _TeamMaster.TeamName = r["TeamName"].ToString();
                                        _TeamMaster.Description = r["Description"].ToString();
                                        _TeamMaster.DefaultDriverId = r["DefaultDriverId"].ToString();
                                        _TeamMaster.DefaultDriverName = r["DefaultDriverName"].ToString();
                                        _TeamMaster.DefaultDriverDate = string.IsNullOrEmpty(r["DefaultDriverDate"].ToString()) ? (DateTime?)null : DateTime.Parse(r["DefaultDriverDate"].ToString());
                                        _TeamMaster.CreatedDate = string.IsNullOrEmpty(r["CreatedDate"].ToString()) ? (DateTime?)null : DateTime.Parse(r["CreatedDate"].ToString());
                                        _TeamMaster.IsMaster = r["IsMaster"].ToString();
                                        lstTeamMaster.Add(_TeamMaster);
                                    }
                                    oLog.WriteToDebugLogFile("Before Serializing the Driver list ", sFuncName);
                                    Context.Response.Output.Write(js.Serialize(lstTeamMaster));
                                    oLog.WriteToDebugLogFile("After Serializing the Driver list, the serialized data is ' " + js.Serialize(lstTeamMaster) + " '", sFuncName);
                                }
                            }
                        }
                        else
                        {
                            List<TeamMaster> lstTeamMaster = new List<TeamMaster>();
                            foreach (DataRow r in ds.Tables[0].Rows)
                            {
                                TeamMaster _TeamMaster = new TeamMaster();
                                _TeamMaster.Id = r["Id"].ToString();
                                _TeamMaster.TeamId = r["TeamId"].ToString();
                                _TeamMaster.TeamName = r["TeamName"].ToString();
                                _TeamMaster.Description = r["Description"].ToString();
                                _TeamMaster.DefaultDriverId = r["DefaultDriverId"].ToString();
                                _TeamMaster.DefaultDriverName = r["DefaultDriverName"].ToString();
                                _TeamMaster.DefaultDriverDate = string.IsNullOrEmpty(r["DefaultDriverDate"].ToString()) ? (DateTime?)null : DateTime.Parse(r["DefaultDriverDate"].ToString());
                                _TeamMaster.CreatedDate = string.IsNullOrEmpty(r["CreatedDate"].ToString()) ? (DateTime?)null : DateTime.Parse(r["CreatedDate"].ToString());
                                _TeamMaster.IsMaster = r["IsMaster"].ToString();
                                lstTeamMaster.Add(_TeamMaster);
                            }
                            oLog.WriteToDebugLogFile("Before Serializing the Driver list ", sFuncName);
                            Context.Response.Output.Write(js.Serialize(lstTeamMaster));
                            oLog.WriteToDebugLogFile("After Serializing the Driver list, the serialized data is ' " + js.Serialize(lstTeamMaster) + " '", sFuncName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void UpdateBulkDriverListByDay(string sOldDriverId, string sNewDriverId, string sCompany, string sDriverDate)
        {
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            try
            {
                sFuncName = "UpdateBulkDriverListByDay()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                oLog.WriteToDebugLogFile("Before Retrieving the Company list", sFuncName);

                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list", sFuncName);
                oLog.WriteToDebugLogFile("Before Calling the method UpdateBulkDriverListbyDay() ", sFuncName);
                int returnResult = oDataAccess.UpdateBulkDriverListbyDay(dsCompanyList, sCompany, sOldDriverId, sNewDriverId, sDriverDate);
                oLog.WriteToDebugLogFile("After Calling the method UpdateBulkDriverListbyDay() ", sFuncName);
                if (returnResult != 0)
                {
                    oLog.WriteToDebugLogFile("Completed With SUCCESS  ", sFuncName);
                    result objResult = new result();
                    objResult.Result = "SUCCESS";
                    objResult.DisplayMessage = "Driver List by Day Updated Successfully";
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
                else
                {
                    oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                    result objResult = new result();
                    objResult.Result = "Error";
                    objResult.DisplayMessage = "Driver List by Day Update Failed";
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void UpdateIndividualDriverListByDay(string sJsonInput, string sCompany)
        {
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            DataTable dtResult = new DataTable();
            try
            {
                sFuncName = "UpdateIndividualDriverListByDay()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                //sJsonInput = "[" + sJsonInput.Trim() + "]";
                //Convert JSON to Array
                oLog.WriteToDebugLogFile("Getting the Json Input from Mobile  '" + sJsonInput + "'", sFuncName);

                oLog.WriteToDebugLogFile("Before Deserialize the Json Input ", sFuncName);
                List<UpdatePostalMaster> PostalMaster = js.Deserialize<List<UpdatePostalMaster>>(sJsonInput);
                oLog.WriteToDebugLogFile("After Deserialize the Json Input ", sFuncName);
                dtResult = ToDataTable<UpdatePostalMaster>(PostalMaster);

                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list", sFuncName);

                oLog.WriteToDebugLogFile("Before Calling the method UpdateIndividualDriverListbyDay() ", sFuncName);
                int returnResult = oDataAccess.UpdateIndividualDriverListbyDay(dsCompanyList, sCompany, dtResult);
                oLog.WriteToDebugLogFile("After Calling the method UpdateIndividualDriverListbyDay() ", sFuncName);
                returnResult = returnResult + 1;
                if (returnResult != 0)
                {
                    oLog.WriteToDebugLogFile("Completed With SUCCESS  ", sFuncName);
                    result objResult = new result();
                    objResult.Result = "SUCCESS";
                    objResult.DisplayMessage = "Driver List by Day Updated Successfully";
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
                else
                {
                    oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                    result objResult = new result();
                    objResult.Result = "Error";
                    objResult.DisplayMessage = "Driver List by Day Update Failed";
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        #endregion

        #region DO Assignment

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void DOA_GetDrivers(string sJsonInput)
        {
            string sFuncName = string.Empty;
            try
            {
                sFuncName = "DOA_GetDrivers()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                string sCompany = string.Empty;

                sJsonInput = "[" + sJsonInput + "]";
                //Split JSON to Individual String
                oLog.WriteToDebugLogFile("Getting the Json Input from Mobile  '" + sJsonInput + "'", sFuncName);
                oLog.WriteToDebugLogFile("Before Deserialize the Json Input ", sFuncName);
                List<Json_TeamMaster> lstDeserialize = js.Deserialize<List<Json_TeamMaster>>(sJsonInput);
                oLog.WriteToDebugLogFile("After Deserialize the Json Input ", sFuncName);
                if (lstDeserialize.Count > 0)
                {
                    Json_TeamMaster objDrivers = lstDeserialize[0];
                    sCompany = objDrivers.sCompany;
                }

                oLog.WriteToDebugLogFile("Before Retrieving the Company list ", sFuncName);

                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list from session ", sFuncName);
                oLog.WriteToDebugLogFile("Before Calling the method Get_Drivers_DOAssignment() ", sFuncName);
                DataSet ds = oDataAccess.Get_Drivers_DOAssignment(dsCompanyList, sCompany);
                oLog.WriteToDebugLogFile("After Calling the method Get_Drivers_DOAssignment() ", sFuncName);
                if (ds != null && ds.Tables.Count > 0)
                {
                    List<DODrivers> lstDrivers = new List<DODrivers>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        DODrivers _Drivers = new DODrivers();
                        _Drivers.Driver = r["Driver"].ToString();
                        _Drivers.Email = r["Email"].ToString();
                        lstDrivers.Add(_Drivers);
                    }
                    oLog.WriteToDebugLogFile("Before Serializing the Drivers list ", sFuncName);
                    Context.Response.Output.Write(js.Serialize(lstDrivers));
                    oLog.WriteToDebugLogFile("After Serializing the Drivers list, the serialized data is ' " + js.Serialize(lstDrivers) + " '", sFuncName);

                }
                else
                {
                    List<DODrivers> lstDrivers = new List<DODrivers>();
                    Context.Response.Output.Write(js.Serialize(lstDrivers));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void DOA_GetNewDrivers(string sJsonInput)
        {
            string sFuncName = string.Empty;
            try
            {
                sFuncName = "DOA_GetNewDrivers()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                string sCompany = string.Empty;

                sJsonInput = "[" + sJsonInput + "]";
                //Split JSON to Individual String
                oLog.WriteToDebugLogFile("Getting the Json Input from Mobile  '" + sJsonInput + "'", sFuncName);
                oLog.WriteToDebugLogFile("Before Deserialize the Json Input ", sFuncName);
                List<Json_TeamMaster> lstDeserialize = js.Deserialize<List<Json_TeamMaster>>(sJsonInput);
                oLog.WriteToDebugLogFile("After Deserialize the Json Input ", sFuncName);
                if (lstDeserialize.Count > 0)
                {
                    Json_TeamMaster objDrivers = lstDeserialize[0];
                    sCompany = objDrivers.sCompany;
                }

                oLog.WriteToDebugLogFile("Before Retrieving the Company list ", sFuncName);

                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list from session ", sFuncName);
                oLog.WriteToDebugLogFile("Before Calling the method Get_NewDrivers_DOAssignment() ", sFuncName);
                DataSet ds = oDataAccess.Get_NewDrivers_DOAssignment(dsCompanyList, sCompany);
                oLog.WriteToDebugLogFile("After Calling the method Get_NewDrivers_DOAssignment() ", sFuncName);
                if (ds != null && ds.Tables.Count > 0)
                {
                    List<NewDODrivers> lstDrivers = new List<NewDODrivers>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        NewDODrivers _Drivers = new NewDODrivers();
                        _Drivers.Type = r["Type"].ToString();
                        _Drivers.Driver = r["Driver"].ToString();
                        lstDrivers.Add(_Drivers);
                    }
                    oLog.WriteToDebugLogFile("Before Serializing the Drivers list ", sFuncName);
                    Context.Response.Output.Write(js.Serialize(lstDrivers));
                    oLog.WriteToDebugLogFile("After Serializing the Drivers list, the serialized data is ' " + js.Serialize(lstDrivers) + " '", sFuncName);

                }
                else
                {
                    List<DODrivers> lstDrivers = new List<DODrivers>();
                    Context.Response.Output.Write(js.Serialize(lstDrivers));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void DOA_GetPostalZoneandAssignedDO(string sJsonInput)
        {
            string sFuncName = string.Empty;
            try
            {
                sFuncName = "DOA_GetPostalZoneandAssignedDO()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                string sCompany = string.Empty;
                string sFromDate = string.Empty;
                string sToDate = string.Empty;

                sJsonInput = "[" + sJsonInput + "]";
                //Split JSON to Individual String
                oLog.WriteToDebugLogFile("Getting the Json Input from Mobile  '" + sJsonInput + "'", sFuncName);
                oLog.WriteToDebugLogFile("Before Deserialize the Json Input ", sFuncName);
                List<Json_PostalZoneAssignedDO> lstDeserialize = js.Deserialize<List<Json_PostalZoneAssignedDO>>(sJsonInput);
                oLog.WriteToDebugLogFile("After Deserialize the Json Input ", sFuncName);
                if (lstDeserialize.Count > 0)
                {
                    Json_PostalZoneAssignedDO objDrivers = lstDeserialize[0];
                    sCompany = objDrivers.sCompany;
                    sFromDate = objDrivers.sFromDate;
                    sToDate = objDrivers.sToDate;
                }

                oLog.WriteToDebugLogFile("Before Retrieving the Company list ", sFuncName);

                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list from session ", sFuncName);
                oLog.WriteToDebugLogFile("Before Calling the method Get_PostalZoneandAssignedDO() ", sFuncName);
                DataSet ds = oDataAccess.Get_PostalZoneandAssignedDO(dsCompanyList, sCompany, sFromDate, sToDate);
                oLog.WriteToDebugLogFile("After Calling the method Get_PostalZoneandAssignedDO() ", sFuncName);
                if (ds != null && ds.Tables.Count > 0)
                {
                    List<PostalZoneandAssignedDO> lstAssignedDO = new List<PostalZoneandAssignedDO>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        PostalZoneandAssignedDO _AssignedDO = new PostalZoneandAssignedDO();
                        _AssignedDO.DriverName = r["DriverName"].ToString();
                        _AssignedDO.PostalZone = r["Postal Zone"].ToString();
                        _AssignedDO.AssignedDO = r["AssignedDO"].ToString();
                        lstAssignedDO.Add(_AssignedDO);
                    }
                    oLog.WriteToDebugLogFile("Before Serializing the Drivers list ", sFuncName);
                    Context.Response.Output.Write(js.Serialize(lstAssignedDO));
                    oLog.WriteToDebugLogFile("After Serializing the Drivers list, the serialized data is ' " + js.Serialize(lstAssignedDO) + " '", sFuncName);

                }
                else
                {
                    List<PostalZoneandAssignedDO> lstAssignedDO = new List<PostalZoneandAssignedDO>();
                    Context.Response.Output.Write(js.Serialize(lstAssignedDO));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void DOA_ReplaceDODrivers(string sJsonInput, string sCompany)
        {
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            DataTable dtResult = new DataTable();
            try
            {
                sFuncName = "DOA_ReplaceDODrivers()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                //sJsonInput = "[" + sJsonInput.Trim() + "]";
                //Convert JSON to Array
                oLog.WriteToDebugLogFile("Getting the Json Input from Mobile  '" + sJsonInput + "'", sFuncName);

                oLog.WriteToDebugLogFile("Before Deserialize the Json Input ", sFuncName);
                List<DOAssignment> DOAssignment = js.Deserialize<List<DOAssignment>>(sJsonInput);
                oLog.WriteToDebugLogFile("After Deserialize the Json Input ", sFuncName);
                dtResult = ToDataTable<DOAssignment>(DOAssignment);

                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list", sFuncName);

                oLog.WriteToDebugLogFile("Before Calling the method ReplaceDODrivers() ", sFuncName);
                int returnResult = oDataAccess.ReplaceDODrivers(dsCompanyList, sCompany, dtResult);
                oLog.WriteToDebugLogFile("After Calling the method ReplaceDODrivers() ", sFuncName);
                if (returnResult != 0)
                {
                    oLog.WriteToDebugLogFile("Completed With SUCCESS  ", sFuncName);
                    result objResult = new result();
                    objResult.Result = "SUCCESS";
                    objResult.DisplayMessage = "DO Drivers Updated Successfully";
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
                else
                {
                    oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                    result objResult = new result();
                    objResult.Result = "Error";
                    objResult.DisplayMessage = "DO Drivers Update Failed";
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void DOA_DOSearch(string sJsonInput)
        {
            string sFuncName = string.Empty;
            try
            {
                sFuncName = "DOA_DOSearch()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                string sCompany = string.Empty;
                string sFromDate = string.Empty;
                string sToDate = string.Empty;
                string sStatus = string.Empty;
                string sDriverName = string.Empty;
                string sTimeofDelivery = string.Empty;

                sJsonInput = "[" + sJsonInput + "]";
                //Split JSON to Individual String
                oLog.WriteToDebugLogFile("Getting the Json Input from Mobile  '" + sJsonInput + "'", sFuncName);
                oLog.WriteToDebugLogFile("Before Deserialize the Json Input ", sFuncName);
                List<Json_DOSearch> lstDeserialize = js.Deserialize<List<Json_DOSearch>>(sJsonInput);
                oLog.WriteToDebugLogFile("After Deserialize the Json Input ", sFuncName);
                if (lstDeserialize.Count > 0)
                {
                    Json_DOSearch objSearch = lstDeserialize[0];
                    sCompany = objSearch.sCompany;
                    sFromDate = objSearch.sFromDate;
                    sToDate = objSearch.sToDate;
                    sStatus = objSearch.sStatus;
                    sDriverName = objSearch.sDriverName;
                    sTimeofDelivery = objSearch.sTimeofDelivery;
                }

                oLog.WriteToDebugLogFile("Before Retrieving the Company list ", sFuncName);

                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list from session ", sFuncName);
                oLog.WriteToDebugLogFile("Before Calling the method DOSearch() ", sFuncName);
                DataSet ds = oDataAccess.DOSearch(dsCompanyList, sCompany, sFromDate, sToDate, sStatus, sDriverName, sTimeofDelivery);
                oLog.WriteToDebugLogFile("After Calling the method DOSearch() ", sFuncName);
                if (ds != null && ds.Tables.Count > 0)
                {
                    List<DOSearch> lstAssignment = new List<DOSearch>();

                    DataSet dsOriginalResult = new DataSet();
                    dsOriginalResult = ds.Copy();
                    DataTable dtRemove = new DataTable();
                    dtRemove = ds.Tables[0];
                    dtRemove.Columns.Remove("ItemCode");
                    dtRemove.Columns.Remove("ItemName");
                    dtRemove.Columns.Remove("QOH");
                    dtRemove.Columns.Remove("StatusTemp");
                    dtRemove.Columns.Remove("Quantity");
                    dtRemove.Columns.Remove("LineRemarks");
                    dtRemove.Columns.Remove("ReturnNoteNo");

                    DataView view = new DataView(dtRemove);
                    DataTable distinctValues = view.ToTable(true, "DocDate", "DONumber", "DocDateTime", "CustomerName", "Address", "Remarks", "Driver", "Email", "ServiceCallID",
                        "SerialNo", "Model", "Priority", "Status", "Printed", "TimeofDelivery");

                    foreach (DataRow r in distinctValues.Rows)
                    {
                        List<Items> lstAttach = new List<Items>();
                        string sItem = "DONumber = '" + r["DONumber"].ToString() + "'";
                        DataView dv = dsOriginalResult.Tables[0].DefaultView;
                        dv.RowFilter = sItem;

                        foreach (DataRowView rowView in dv)
                        {

                            DataRow row = rowView.Row;
                            if (row["ItemCode"].ToString() != string.Empty)
                            {
                                Items _Items = new Items();
                                _Items.ItemCode = row["ItemCode"].ToString();
                                _Items.ItemName = row["ItemName"].ToString();
                                _Items.QOH = row["QOH"].ToString();
                                _Items.StatusTemp = row["StatusTemp"].ToString();
                                _Items.LineRemarks = row["LineRemarks"].ToString();
                                _Items.ReturnNoteNo = row["ReturnNoteNo"].ToString();
                                _Items.Quantity = row["Quantity"].ToString();
                                lstAttach.Add(_Items);
                            }
                        }

                        DOSearch _Assignment = new DOSearch();
                        _Assignment.DocDate = r["DocDate"].ToString();
                        _Assignment.DONumber = r["DONumber"].ToString();
                        _Assignment.DocDateTime = r["DocDateTime"].ToString();
                        _Assignment.CustomerName = r["CustomerName"].ToString();
                        _Assignment.Address = r["Address"].ToString();
                        _Assignment.Remarks = r["Remarks"].ToString();
                        _Assignment.Driver = r["Driver"].ToString();
                        _Assignment.Email = r["Email"].ToString();
                        _Assignment.ServiceCallID = r["ServiceCallID"].ToString();
                        _Assignment.SerialNo = r["SerialNo"].ToString();
                        _Assignment.Model = r["Model"].ToString();
                        _Assignment.Priority = r["Priority"].ToString();
                        _Assignment.Status = r["Status"].ToString();
                        _Assignment.Printed = r["Printed"].ToString();
                        _Assignment.TimeofDelivery = r["TimeofDelivery"].ToString();
                        _Assignment.ItemsArray = lstAttach;
                        lstAssignment.Add(_Assignment);
                    }
                    oLog.WriteToDebugLogFile("Before Serializing the Drivers list ", sFuncName);
                    Context.Response.Output.Write(js.Serialize(lstAssignment));
                    oLog.WriteToDebugLogFile("After Serializing the Drivers list, the serialized data is ' " + js.Serialize(lstAssignment) + " '", sFuncName);
                }
                else
                {
                    List<DOSearch> lstAssignment = new List<DOSearch>();
                    Context.Response.Output.Write(js.Serialize(lstAssignment));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void DOA_UpdateDO(string sJsonInput, string sCompany)
        {
            string sFuncName = string.Empty;
            SAPbobsCOM.Documents oDOAssignment;
            int iResult = 0;
            string sEmailResult = string.Empty;
            int iEmailCount = 0;
            double lRetCode;

            try
            {
                sFuncName = "DOA_UpdateDO()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                //Split JSON to Individual String
                oLog.WriteToDebugLogFile("Getting the Json Input from Mobile  '" + sJsonInput + "'", sFuncName);
                oLog.WriteToDebugLogFile("Before Deserialize the Json Input ", sFuncName);
                List<DOSearch> lstDeserialize = js.Deserialize<List<DOSearch>>(sJsonInput);
                oLog.WriteToDebugLogFile("After Deserialize the Json Input ", sFuncName);

                oLog.WriteToDebugLogFile("Before Retrieving the Company list", sFuncName);

                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list", sFuncName);
                oDICompany = oDataAccess.ConnectToTargetCompany(sCompany);
                SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                for (int i = 0; i <= lstDeserialize.Count - 1; i++)
                {
                    ////Using DIAPI Update the DocDueDate in ODLN
                    if (lstDeserialize[i].Status != "Closed")
                    {
                        oLog.WriteToDebugLogFile("Connecting to target company ", sFuncName);


                        string sQuery = "SELECT TOP 1 Convert(varchar(10),DocDueDate,101) [DocDate],DocEntry from ODLN Where DocNum = " + lstDeserialize[i].DONumber + "";
                        oLog.WriteToDebugLogFile(sQuery, sFuncName);

                        oRS.DoQuery(sQuery);

                        oLog.WriteToDebugLogFile("Left Side Date :" + oRS.Fields.Item("DocDate").Value, sFuncName);
                        oLog.WriteToDebugLogFile("Right Side Date: " + lstDeserialize[i].DocDate.Trim(), sFuncName);
                        if (Convert.ToString(oRS.Fields.Item("DocDate").Value) != lstDeserialize[i].DocDate.Trim())
                        {
                            oLog.WriteToDebugLogFile("Getting the Business Object ", sFuncName);
                            oDOAssignment = (SAPbobsCOM.Documents)(oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDeliveryNotes));

                            if (oDOAssignment.GetByKey(Convert.ToInt32(oRS.Fields.Item("DocEntry").Value)))
                            {
                                oDOAssignment.DocDueDate = Convert.ToDateTime(lstDeserialize[i].DocDate.ToString());
                            }
                            lRetCode = oDOAssignment.Update();
                        }
                    }

                    //Using Normal SQL, Update the DriverName , Delivery Status, Time of Delivery and Line Remarks
                    oLog.WriteToDebugLogFile("Before Calling the method UpdateDO() ", sFuncName);
                    DataTable dtItemArray = ToDataTable<Items>(lstDeserialize[i].ItemsArray);
                    int returnResult = oDataAccess.UpdateDO(dsCompanyList, sCompany, lstDeserialize[i].DocDate, lstDeserialize[i].DONumber, lstDeserialize[i].Driver, lstDeserialize[i].TimeofDelivery, dtItemArray);
                    oLog.WriteToDebugLogFile("After Calling the method UpdateDO() ", sFuncName);

                    //Send Email if the Delivery Status like 'Cancelled'
                    StringBuilder filter = new StringBuilder();
                    filter.Append("StatusTemp Like '%Cancelled%'");
                    DataView dv = dtItemArray.DefaultView;
                    dv.RowFilter = filter.ToString();
                    dtItemArray = dv.ToTable();

                    if (dtItemArray.Rows.Count > 0)
                    {
                        int emailCount = 0;
                        foreach (DataRow item in dtItemArray.Rows)
                        {
                            // Check the email sent status UDF in Line Level for sending Email
                            string sQueryforDocEntry1 = "SELECT TOP 1 U_OB_EmailSentStatus EmailStatus from DLN1 Where ItemCode = '" + item["ItemCode"].ToString() + "' and DocEntry in (Select DocEntry from ODLN where DocNum = " + lstDeserialize[i].DONumber + ")";
                            oLog.WriteToDebugLogFile("Checking the Email Sent Status UDF : " + sQueryforDocEntry1, sFuncName);

                            oRS.DoQuery(sQueryforDocEntry1);

                            if (Convert.ToString(oRS.Fields.Item("EmailStatus").Value) == "Y")
                            {
                                emailCount = emailCount + 1;
                            }
                        }

                        if (emailCount == 0)
                        {

                            //1) Return DO Note if the Delivery Status like 'Cancelled'

                            string sQueryforDocEntry = "SELECT TOP 1 DocEntry from ODLN Where DocNum = " + lstDeserialize[i].DONumber + "";
                            oLog.WriteToDebugLogFile(sQueryforDocEntry, sFuncName);

                            oRS.DoQuery(sQueryforDocEntry);

                            oLog.WriteToDebugLogFile("Before Calling the method CreateDOReturn() ", sFuncName);
                            long lReturnResult = oDataAccess.CreateDOReturn(Convert.ToString(oRS.Fields.Item("DocEntry").Value), Convert.ToInt32(lstDeserialize[i].ServiceCallID == string.Empty ? "0" : lstDeserialize[i].ServiceCallID), oDICompany, ref sErrDesc);
                            oLog.WriteToDebugLogFile("After Calling the method CreateDOReturn() ", sFuncName);
                            if (lReturnResult > 1) // this step is to update the return Document Number in the UDF
                            {
                                oLog.WriteToDebugLogFile("Before Updating the Return Note UDF by Calling the method UpdateReturnNoteUDF() ", sFuncName);
                                int UpdateReturnNoteUDF = oDataAccess.Update_ReturnNoteUDF(dsCompanyList, sCompany, lstDeserialize[i].DONumber, lReturnResult);
                                oLog.WriteToDebugLogFile("After Updating the Return Note UDF by Calling the method UpdateReturnNoteUDF() ", sFuncName);

                                //2) Send the email for the Delivery Status like 'Cancelled'
                                oLog.WriteToDebugLogFile("Before Calling the method SendEmail() ", sFuncName);

                                long lEmailSentStatus = oSendDOEmail.SendEmail(dtItemArray, lstDeserialize[i].Email, lstDeserialize[i].ServiceCallID, lstDeserialize[i].DONumber, lReturnResult, ref sErrDesc);
                                if (lEmailSentStatus == 0)
                                {
                                    oLog.WriteToDebugLogFile("Before Updating the Email Sent Status UDF() ", sFuncName);
                                    iEmailCount = iEmailCount + 1;
                                    // update the Email sent status UDF in line level
                                    int emailSentStauts = oDataAccess.UpdateEmailSentStatus(dsCompanyList, sCompany, lstDeserialize[i].DONumber, dtItemArray);
                                    oLog.WriteToDebugLogFile("After Updating the Email Sent Status UDF() ", sFuncName);
                                }
                                oLog.WriteToDebugLogFile("After Calling the method SendEmail() ", sFuncName);
                            }

                            if (lReturnResult == 1)
                            {
                                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                                result objResult = new result();
                                objResult.Result = "ERROR";
                                objResult.DisplayMessage = sErrDesc;
                                lstResult.Add(objResult);
                                Context.Response.Output.Write(js.Serialize(lstResult));
                                return;
                            }
                        }
                    }
                    iResult = iResult + returnResult;
                }

                if (iResult != 0)
                {
                    oLog.WriteToDebugLogFile("Completed With SUCCESS  ", sFuncName);
                    result objResult = new result();
                    objResult.Result = "SUCCESS";
                    if (iEmailCount == 0)
                    {
                        objResult.DisplayMessage = "DO Updated Successfully";
                    }
                    else
                    {
                        objResult.DisplayMessage = "DO Updated and Email sent Successfully";
                    }
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
                else
                {
                    oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                    result objResult = new result();
                    objResult.Result = "Error";
                    objResult.DisplayMessage = "DO Update Failed";
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public void DOA_CreatePDF(string sDocNum, string sCompany)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            string sProcName = string.Empty;
            DataView oDTView = new DataView();
            try
            {
                sFuncName = "DOA_CreatePDF";

                oLog.WriteToDebugLogFile("Before Retrieving the Company list ", sFuncName);
                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list ", sFuncName);

                if (dsCompanyList != null && dsCompanyList.Tables.Count > 0)
                {
                    oDTView = dsCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        oLog.WriteToDebugLogFile("Before Creating PDF file for DocNum : " + sDocNum, sFuncName);
                        //Create PDF file

                        string sFileName = "/PDF/DO/" + sDocNum + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + DateTime.Now.Millisecond + ".pdf";
                        string directory = HttpContext.Current.Server.MapPath("TEMP") + "/PDF/DO";
                        string AttachFile = HttpContext.Current.Server.MapPath("TEMP") + sFileName;
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        ReportDocument myReportDocument;
                        myReportDocument = new ReportDocument();
                        myReportDocument.Load(Server.MapPath("~/Reports/") + "VS_SP010_Web_RPT_DOCollectionNoteMain.rpt");
                        myReportDocument.SetDatabaseLogon(oDTView[0]["U_DBUserName"].ToString(), oDTView[0]["U_DBPassword"].ToString(), oDTView[0]["U_Server"].ToString(), oDTView[0]["U_DBName"].ToString());

                        CrystalDecisions.Shared.ParameterValues pval1 = new ParameterValues();

                        ParameterDiscreteValue pdisval1 = new ParameterDiscreteValue();

                        pdisval1.Value = sDocNum;
                        pval1.Add(pdisval1);

                        myReportDocument.DataDefinition.ParameterFields["@DocNum"].ApplyCurrentValues(pval1);

                        ExportOptions CrExportOptions = default(ExportOptions);
                        DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
                        ExcelFormatOptions CrExcelFormat = new ExcelFormatOptions();
                        PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();
                        ExcelFormatOptions CrExcelTypeOptions = new ExcelFormatOptions();

                        CrDiskFileDestinationOptions.DiskFileName = AttachFile;
                        CrExportOptions = myReportDocument.ExportOptions;
                        var _with1 = CrExportOptions;
                        _with1.ExportDestinationType = ExportDestinationType.DiskFile;
                        _with1.ExportFormatType = ExportFormatType.PortableDocFormat;
                        _with1.DestinationOptions = CrDiskFileDestinationOptions;
                        _with1.FormatOptions = CrFormatTypeOptions;
                        myReportDocument.Export();

                        oLog.WriteToDebugLogFile("PDF Created successfully for Doc Num : " + sDocNum, sFuncName);
                        result objResult = new result();
                        objResult.Result = "Success";
                        objResult.DisplayMessage = "/TEMP" + sFileName;
                        lstResult.Add(objResult);
                        Context.Response.Output.Write(js.Serialize(lstResult));
                    }
                }
                else
                {
                    oLog.WriteToErrorLogFile("There is No Company List in the Holding Company", sFuncName);
                    oLog.WriteToDebugLogFile("Completed With ERROR : There is No Company List in the Holding Company ", sFuncName);
                    result objResult = new result();
                    objResult.Result = "Failure";
                    objResult.DisplayMessage = "There is No Company List in the Holding Company";
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
        public void DOA_Print_UpdateUDF(string sDocNum, string sCompany)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            string sProcName = string.Empty;
            DataView oDTView = new DataView();
            try
            {
                sFuncName = "DOA_Print_UpdateUDF";

                oLog.WriteToDebugLogFile("Before Retrieving the Company list ", sFuncName);
                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list ", sFuncName);

                int returnResult = oDataAccess.Update_PrintUDF(dsCompanyList, sCompany, sDocNum);

                if (returnResult != 0)
                {
                    oLog.WriteToDebugLogFile("Successfully Updated for : " + sDocNum, sFuncName);
                    result objResult = new result();
                    objResult.Result = "Success";
                    objResult.DisplayMessage = "Updated Successfully";
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
                else
                {
                    oLog.WriteToDebugLogFile("Update Fail for : " + sDocNum, sFuncName);
                    result objResult = new result();
                    objResult.Result = "Failure";
                    objResult.DisplayMessage = "Update Fail";
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        #endregion

        #region Company Connection

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void ConnectCompany(string sCompanyDB)
        {
            string sFuncName = string.Empty;
            SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();
            try
            {
                sFuncName = "ConnectCompany";

                oLog.WriteToDebugLogFile("Before Connecting to the SAP Company ", sFuncName);
                oCompany = oDataAccess.ConnectToTargetCompany(sCompanyDB);
                oLog.WriteToDebugLogFile("After Connecting to the SAP Company ", sFuncName);
                result objResult = new result();
                objResult.Result = "SUCCESS";
                objResult.DisplayMessage = "Company Connected Successfully";
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
            catch (Exception ex)
            {
                result objResult = new result();
                objResult.Result = "ERROR";
                objResult.DisplayMessage = ex.Message.ToString();
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }

        }
        #endregion

        #region Email Test

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void TestEmail(string sJsonInput, string sToEmailId)
        {
            string sFuncName = string.Empty;
            result objResult = new result();
            try
            {
                sFuncName = "TestEmail";
                SendDOEmail objMail = new SendDOEmail();
                oLog.WriteToDebugLogFile("Before Calling the Test mail Function", sFuncName);
                List<DOSearch> lstDeserialize = js.Deserialize<List<DOSearch>>(sJsonInput);
                DataTable dtItemArray = ToDataTable<Items>(lstDeserialize[0].ItemsArray);
                StringBuilder filter = new StringBuilder();
                filter.Append("StatusTemp Like '%Cancelled%'");
                DataView dv = dtItemArray.DefaultView;
                dv.RowFilter = filter.ToString();
                dtItemArray = dv.ToTable();
                if (dtItemArray.Rows.Count > 0)
                {
                    if (sToEmailId != string.Empty)
                    {
                        long lReturnResult = objMail.SendEmail(dtItemArray, sToEmailId, "1520135", "9578652", 12345, ref sErrDesc);
                        oLog.WriteToDebugLogFile("After Calling the Test mail Function ", sFuncName);


                        if (sErrDesc == string.Empty)
                        {
                            objResult.Result = "SUCCESS";
                            objResult.DisplayMessage = "Email Send Successfully";
                        }
                        else
                        {
                            objResult.Result = "ERROR";
                            objResult.DisplayMessage = sErrDesc;
                        }
                    }
                    else
                    {
                        objResult.Result = "ERROR";
                        objResult.DisplayMessage = "Kindly Provide the Email Id";
                    }
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
                else
                {
                    objResult.Result = "ERROR";
                    objResult.DisplayMessage = "No Delivery Stauts like cancelled to send email";
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
            }
            catch (Exception ex)
            {
                objResult.Result = "ERROR";
                objResult.DisplayMessage = ex.Message.ToString();
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        #endregion

        #region Other DO & Collection Note

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void DOA_SearchOtherDOandCN(string sJsonInput, string sCompany)
        {
            string sFuncName = string.Empty;
            Json_SearchOtherDOandCN objSearchDO = new Json_SearchOtherDOandCN();
            try
            {
                sFuncName = "DOA_SearchOtherDOandCN()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);


                sJsonInput = "[" + sJsonInput + "]";
                //Split JSON to Individual String
                oLog.WriteToDebugLogFile("Getting the Json Input from Mobile  '" + sJsonInput + "'", sFuncName);
                oLog.WriteToDebugLogFile("Before Deserialize the Json Input ", sFuncName);
                List<Json_SearchOtherDOandCN> lstDeserialize = js.Deserialize<List<Json_SearchOtherDOandCN>>(sJsonInput);
                oLog.WriteToDebugLogFile("After Deserialize the Json Input ", sFuncName);
                if (lstDeserialize.Count > 0)
                {
                    objSearchDO = lstDeserialize[0];
                }

                oLog.WriteToDebugLogFile("Before Retrieving the Company list ", sFuncName);

                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list from session ", sFuncName);
                oLog.WriteToDebugLogFile("Before Calling the method SearchOtherDOandCN() ", sFuncName);
                DataSet ds = oDataAccess.SearchOtherDOandCN(dsCompanyList, sCompany, objSearchDO.sFromDate, objSearchDO.sToDate, objSearchDO.sCustName,
                    objSearchDO.sDocNum == null ? "" : objSearchDO.sDocNum,
                    objSearchDO.sDriverIncharge == null ? "" : objSearchDO.sDriverIncharge,
                    objSearchDO.sStatus == null ? "" : objSearchDO.sStatus,
                    objSearchDO.sType == null ? "" : objSearchDO.sType);
                oLog.WriteToDebugLogFile("After Calling the method SearchOtherDOandCN() ", sFuncName);
                if (ds != null && ds.Tables.Count > 0)
                {
                    List<OtherDOSearch> lstOtherDOSearch = new List<OtherDOSearch>();

                    DataSet dsOriginalResult = new DataSet();
                    dsOriginalResult = ds.Copy();
                    DataTable dtRemove = new DataTable();
                    dtRemove = ds.Tables[0];
                    dtRemove.Columns.Remove("LineTable");
                    dtRemove.Columns.Remove("ItemCode");
                    dtRemove.Columns.Remove("ItemName");
                    dtRemove.Columns.Remove("SerialNumber");
                    dtRemove.Columns.Remove("Quantity");
                    dtRemove.Columns.Remove("Warehouse");
                    dtRemove.Columns.Remove("DeliveryStatus");
                    dtRemove.Columns.Remove("LineRemarks");

                    DataView view = new DataView(dtRemove);
                    DataTable distinctValues = view.ToTable(true, "Type", "ServiceCallId", "DocEntry", "DocNum", "DocType", "DocDate", "CustName", "Remarks", "LiftAccess", "Address", "DriverInCharge", "DriverInCharge1", "DriverInCharge2", "DeliveryTime", "HeaderTable");

                    foreach (DataRow r in distinctValues.Rows)
                    {
                        List<OtherDOSearchItems> lstAttach = new List<OtherDOSearchItems>();
                        string sItem = "DocNum = '" + r["DocNum"].ToString() + "' and DocEntry = '" + r["DocEntry"].ToString() + "' and DocType = '" + r["DocType"].ToString() + "'";
                        DataView dv = dsOriginalResult.Tables[0].DefaultView;
                        dv.RowFilter = sItem;

                        foreach (DataRowView rowView in dv)
                        {

                            DataRow row = rowView.Row;
                            if (row["ItemCode"].ToString() != string.Empty)
                            {
                                OtherDOSearchItems _Items = new OtherDOSearchItems();
                                _Items.LineTable = row["LineTable"].ToString();
                                _Items.ItemCode = row["ItemCode"].ToString();
                                _Items.ItemName = row["ItemName"].ToString();
                                _Items.SerialNumber = row["SerialNumber"].ToString();
                                _Items.Quantity = row["Quantity"].ToString();
                                _Items.Warehouse = row["Warehouse"].ToString();
                                _Items.DeliveryStatus = row["DeliveryStatus"].ToString();
                                _Items.LineRemarks = row["LineRemarks"].ToString();
                                lstAttach.Add(_Items);
                            }
                        }

                        OtherDOSearch _SearchItems = new OtherDOSearch();
                        _SearchItems.Type = r["Type"].ToString();
                        _SearchItems.ServiceCallId = r["ServiceCallId"].ToString();
                        _SearchItems.DocEntry = r["DocEntry"].ToString();
                        _SearchItems.DocNum = r["DocNum"].ToString();
                        _SearchItems.DocType = r["DocType"].ToString();
                        _SearchItems.DocDate = r["DocDate"].ToString();
                        _SearchItems.CustName = r["CustName"].ToString();
                        _SearchItems.Remarks = r["Remarks"].ToString();
                        _SearchItems.LiftAccess = r["LiftAccess"].ToString();
                        _SearchItems.Address = r["Address"].ToString();
                        _SearchItems.DriverInCharge1 = r["DriverInCharge"].ToString();
                        _SearchItems.DriverInCharge2 = r["DriverInCharge1"].ToString();
                        _SearchItems.DriverInCharge3 = r["DriverInCharge2"].ToString();
                        _SearchItems.DeliveryTime = r["DeliveryTime"].ToString();
                        _SearchItems.HeaderTable = r["HeaderTable"].ToString();
                        _SearchItems.ItemsArray = lstAttach;
                        lstOtherDOSearch.Add(_SearchItems);
                    }
                    oLog.WriteToDebugLogFile("Before Serializing the Drivers list ", sFuncName);
                    Context.Response.Output.Write(js.Serialize(lstOtherDOSearch));
                    oLog.WriteToDebugLogFile("After Serializing the Drivers list, the serialized data is ' " + js.Serialize(lstOtherDOSearch) + " '", sFuncName);
                }
                else
                {
                    List<OtherDOSearch> lstSearch = new List<OtherDOSearch>();
                    Context.Response.Output.Write(js.Serialize(lstSearch));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void DOA_UpdateOtherDOandCN(string sJsonInput, string sCompany)
        {
            string sFuncName = string.Empty;
            int returnResult = 0;
            try
            {
                sFuncName = "DOA_UpdateOtherDOandCN()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                //Split JSON to Individual String
                oLog.WriteToDebugLogFile("Getting the Json Input from Mobile  '" + sJsonInput + "'", sFuncName);
                oLog.WriteToDebugLogFile("Before Deserialize the Json Input ", sFuncName);
                List<OtherDOSearch> lstDeserialize = js.Deserialize<List<OtherDOSearch>>(sJsonInput);
                oLog.WriteToDebugLogFile("After Deserialize the Json Input ", sFuncName);

                oLog.WriteToDebugLogFile("Before Retrieving the Company list", sFuncName);

                DataSet dsCompanyList = oDataAccess.Get_CompanyList();
                oLog.WriteToDebugLogFile("After Retrieving the Company list", sFuncName);

                for (int i = 0; i <= lstDeserialize.Count - 1; i++)
                {
                    oLog.WriteToDebugLogFile("Before Calling the method UpdateDO() ", sFuncName);
                    DataTable dtItemArray = ToDataTable<OtherDOSearchItems>(lstDeserialize[i].ItemsArray);
                    returnResult = oDataAccess.UpdateOtherDOandCN(dsCompanyList, sCompany, lstDeserialize[i].DocEntry, lstDeserialize[i].DocNum, lstDeserialize[i].DriverInCharge1, lstDeserialize[i].DriverInCharge2, lstDeserialize[i].DriverInCharge3, lstDeserialize[i].DeliveryTime, lstDeserialize[i].HeaderTable, dtItemArray);
                    oLog.WriteToDebugLogFile("After Calling the method UpdateDO() ", sFuncName);
                }
                if (returnResult > 0)
                {
                    oLog.WriteToDebugLogFile("Completed With SUCCESS  ", sFuncName);
                    result objResult = new result();
                    objResult.Result = "SUCCESS";
                    objResult.DisplayMessage = "DO Updated Successfully";
                    lstResult.Add(objResult);
                    Context.Response.Output.Write(js.Serialize(lstResult));
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                result objResult = new result();
                objResult.Result = "Error";
                objResult.DisplayMessage = sErrDesc;
                lstResult.Add(objResult);
                Context.Response.Output.Write(js.Serialize(lstResult));
            }
        }

        #endregion

        #endregion

        #region Classes

        class result
        {
            public string Result { get; set; }
            public string DisplayMessage { get; set; }
        }

        class Company
        {
            public string U_DBName { get; set; }
            public string U_CompName { get; set; }
            public string U_SAPUserName { get; set; }
            public string U_SAPPassword { get; set; }
            public string U_DBUserName { get; set; }
            public string U_DBPassword { get; set; }
            public string U_ConnString { get; set; }
            public string U_Server { get; set; }
            public string U_LicenseServer { get; set; }
        }

        class UserInfo
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string CompanyCode { get; set; }
            public string Message { get; set; }
        }

        class Json_UserInfo
        {
            public string sUserName { get; set; }
            public string sPassword { get; set; }
            public string sCompany { get; set; }
        }

        class Json_TeamMaster
        {
            public string sCompany { get; set; }
            public string sDriverDate { get; set; }
        }

        class Json_SearchOtherDOandCN
        {
            public string sFromDate { get; set; }
            public string sToDate { get; set; }
            public string sCustName { get; set; }
            public string sDocNum { get; set; }
            public string sDriverIncharge { get; set; }
            public string sStatus { get; set; }
            public string sType { get; set; }
        }

        class Json_PostalZoneAssignedDO
        {
            public string sCompany { get; set; }
            public string sFromDate { get; set; }
            public string sToDate { get; set; }
        }

        class Json_DOSearch
        {
            public string sCompany { get; set; }
            public string sFromDate { get; set; }
            public string sToDate { get; set; }
            public string sStatus { get; set; }
            public string sDriverName { get; set; }
            public string sTimeofDelivery { get; set; }
        }

        class Drivers
        {
            public string DriverId { get; set; }
            public string DriverName { get; set; }
        }

        class DODrivers
        {
            public string Driver { get; set; }
            public string Email { get; set; }
        }

        class NewDODrivers
        {
            public string Type { get; set; }
            public string Driver { get; set; }
        }

        class PostalZoneandAssignedDO
        {
            public string DriverName { get; set; }
            public string PostalZone { get; set; }
            public string AssignedDO { get; set; }
        }

        class TeamMaster
        {
            public string Id { get; set; }
            public string TeamId { get; set; }
            public string TeamName { get; set; }
            public string Description { get; set; }
            public string DefaultDriverId { get; set; }
            public string DefaultDriverName { get; set; }
            public DateTime? DefaultDriverDate { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string IsMaster { get; set; }
        }

        class UpdatePostalMaster
        {
            public string Id { get; set; }
            public string TeamId { get; set; }
            public string TeamName { get; set; }
            public string Description { get; set; }
            public string NewDriverId { get; set; }
            public string DefaultDriverId { get; set; }
            public string DefaultDriverName { get; set; }
            public DateTime? DefaultDriverDate { get; set; }
            public DateTime? CreatedDate { get; set; }
        }

        class DOAssignment
        {
            public string DocDate { get; set; }
            public string DONumber { get; set; }
            public string CustomerName { get; set; }
            public string Address { get; set; }
            public string Remarks { get; set; }
            public string Driver { get; set; }
            public string ServiceCallID { get; set; }
            public string SerialNo { get; set; }
            public string Model { get; set; }
            public string Priority { get; set; }
            public string Status { get; set; }
            //public string ItemCode { get; set; }
            //public string ItemName { get; set; }
            //public string QOH { get; set; }
            //public string StatusTemp { get; set; }
            //public string Quantity { get; set; }
        }

        class DOSearch
        {
            public string DocDate { get; set; }
            public string DONumber { get; set; }
            public string DocDateTime { get; set; }
            public string CustomerName { get; set; }
            public string Address { get; set; }
            public string Remarks { get; set; }
            public string Driver { get; set; }
            public string Email { get; set; }
            public string ServiceCallID { get; set; }
            public string SerialNo { get; set; }
            public string Model { get; set; }
            public string Priority { get; set; }
            public string Status { get; set; }
            public string Printed { get; set; }
            public string TimeofDelivery { get; set; }
            public List<Items> ItemsArray { get; set; }
            //public string ItemCode { get; set; }
            //public string ItemName { get; set; }
            //public string QOH { get; set; }
            //public string StatusTemp { get; set; }
            //public string Quantity { get; set; }
        }

        class Items
        {
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public string QOH { get; set; }
            public string StatusTemp { get; set; }
            public string ReturnNoteNo { get; set; }
            public string LineRemarks { get; set; }
            public string Quantity { get; set; }
        }

        class OtherDOSearch
        {
            public string Type { get; set; }
            public string ServiceCallId { get; set; }
            public string DocEntry { get; set; }
            public string DocNum { get; set; }
            public string DocType { get; set; }
            public string DocDate { get; set; }
            public string CustName { get; set; }
            public string Remarks { get; set; }
            public string LiftAccess { get; set; }
            public string Address { get; set; }
            public string DriverInCharge1 { get; set; }
            public string DriverInCharge2 { get; set; }
            public string DriverInCharge3 { get; set; }
            public string DeliveryTime { get; set; }
            public string HeaderTable { get; set; }
            public List<OtherDOSearchItems> ItemsArray { get; set; }
        }

        class OtherDOSearchItems
        {
            public string LineTable { get; set; }
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public string SerialNumber { get; set; }
            public string Quantity { get; set; }
            public string Warehouse { get; set; }
            public string DeliveryStatus { get; set; }
            public string LineRemarks { get; set; }
        }

        #endregion

        #region public methods

        public DataTable JsonStringToDataTable(string jsonString)
        {
            DataTable dt = new DataTable();
            string sFuncName = string.Empty;
            try
            {
                sFuncName = "JsonStringToDataTable()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
                List<string> ColumnsName = new List<string>();
                foreach (string jSA in jsonStringArray)
                {
                    string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                    foreach (string ColumnsNameData in jsonStringData)
                    {
                        try
                        {
                            int idx = ColumnsNameData.IndexOf(":");
                            string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                            if (!ColumnsName.Contains(ColumnsNameString.Trim()))
                            {
                                ColumnsName.Add(ColumnsNameString.Trim());
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                        }
                    }
                    break;
                }
                foreach (string AddColumnName in ColumnsName)
                {
                    if (AddColumnName.Contains("Date"))
                    { dt.Columns.Add(AddColumnName, typeof(DateTime)); }
                    else
                    { dt.Columns.Add(AddColumnName); }

                }
                foreach (string jSA in jsonStringArray)
                {
                    string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                    DataRow nr = dt.NewRow();
                    foreach (string rowData in RowData)
                    {
                        try
                        {
                            int idx = rowData.Trim().IndexOf(":");
                            string RowColumns = rowData.Trim().Substring(0, idx - 1).Replace("\"", "");
                            string RowDataString = rowData.Trim().Substring(idx + 1).Replace("\"", "");
                            nr[RowColumns] = RowDataString.Trim();
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                    dt.Rows.Add(nr);
                }
            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
            }
            return dt;
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        #endregion
    }
}
