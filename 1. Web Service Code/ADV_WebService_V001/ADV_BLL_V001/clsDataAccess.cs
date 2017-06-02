using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace ADV_WebService_V001
{
    public class clsDataAccess
    {
        clsLog oLog = new clsLog();
        public string sErrDesc = string.Empty;
        SAPbobsCOM.Company oDICompany;

        public static string ConnectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;

        #region Login
        public DataSet Get_CompanyList()
        {
            DataSet oDataset;
            string sFuncName = string.Empty;
            string sProcName = string.Empty;

            try
            {
                sFuncName = "Get_CompanyList()";
                sProcName = "VS_SP001_Web_GetCompanyList";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                oLog.WriteToDebugLogFile("Calling Run_StoredProcedure() " + sProcName, sFuncName);

                oDataset = SqlHelper.ExecuteDataSet(ConnectionString, CommandType.StoredProcedure, sProcName);

                oLog.WriteToDebugLogFile("Completed With SUCCESS  ", sFuncName);

                return oDataset;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        public DataSet LoginValidation(DataSet oDTCompanyList, string sUserName, string sPassword, string sCompany)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            string sProcName = string.Empty;
            DataView oDTView = new DataView();

            try
            {
                sFuncName = "LoginValidation()";
                sProcName = "VS_SP002_Web_LoginValidation";

                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                oLog.WriteToDebugLogFile("Calling Run_StoredProcedure() " + sProcName, sFuncName);

                if (oDTCompanyList != null && oDTCompanyList.Tables.Count > 0)
                {
                    oDTView = oDTCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        oDataset = SqlHelper.ExecuteDataSet(oDTView[0]["U_ConnString"].ToString(), CommandType.StoredProcedure, sProcName,
                            Data.CreateParameter("@UserCode", sUserName), Data.CreateParameter("@Password", sPassword), Data.CreateParameter("@Company", sCompany));
                        oLog.WriteToDebugLogFile("Completed With SUCCESS  ", sFuncName);
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return oDataset;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return oDataset;
                }

                return oDataset;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }
        #endregion

        #region Postal Zone master
        public DataSet Get_Drivers(DataSet oDTCompanyList, string sCompany)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            string sProcName = string.Empty;
            DataView oDTView = new DataView();

            try
            {
                sFuncName = "Get_Drivers()";
                sProcName = "VS_SP003_Web_GetDriver";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                oLog.WriteToDebugLogFile("Calling Run_StoredProcedure() " + sProcName, sFuncName);
                if (oDTCompanyList != null && oDTCompanyList.Tables.Count > 0)
                {
                    oDTView = oDTCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        oDataset = SqlHelper.ExecuteDataSet(oDTView[0]["U_ConnString"].ToString(), CommandType.StoredProcedure, sProcName,
                            Data.CreateParameter("@Company", sCompany));

                        oLog.WriteToDebugLogFile("Completed With SUCCESS ", sFuncName);
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return oDataset;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return oDataset;
                }
                return oDataset;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        public DataSet Get_TeamMaster(DataSet oDTCompanyList, string sCompany, string sDriverDate)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            string sProcName = string.Empty;
            DataView oDTView = new DataView();

            try
            {
                sFuncName = "Get_TeamMaster()";
                sProcName = "VS_SP004_Web_GetTeamMaster";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                oLog.WriteToDebugLogFile("Calling Run_StoredProcedure() " + sProcName, sFuncName);
                if (oDTCompanyList != null && oDTCompanyList.Tables.Count > 0)
                {
                    oDTView = oDTCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        oDataset = SqlHelper.ExecuteDataSet(oDTView[0]["U_ConnString"].ToString(), CommandType.StoredProcedure, sProcName,
                            Data.CreateParameter("@Company", sCompany), Data.CreateParameter("@DriverDate", sDriverDate));

                        oLog.WriteToDebugLogFile("Completed With SUCCESS ", sFuncName);
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return oDataset;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return oDataset;
                }
                return oDataset;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        public int UpdateBulkPostalZoneMaster(DataSet oDTCompanyList, string sCompany, string sOldDriverId, string sNewDriverId)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            int returnresult = 0;
            try
            {
                sFuncName = "UpdateBulkPostalZoneMaster()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                if (oDTCompanyList != null && oDTCompanyList.Tables.Count > 0)
                {
                    oDTView = oDTCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        using (SqlConnection connection = new SqlConnection(oDTView[0]["U_ConnString"].ToString()))
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "Declare @NewDriverName varchar(max);set @NewDriverName = (select top 1 jobTitle from OHEM with (NOLOCK) where empID = @sNewDriverId);UPDATE [ADV_PostalZoneMaster] set DefaultDriverId = @sNewDriverId , DefaultDriverName = @NewDriverName where DefaultDriverId = @sOldDriverId and IsNull(DefaultDriverDate,'') = '' AND IsMaster = 0 ";

                            command.Parameters.AddWithValue("@sOldDriverId", sOldDriverId);
                            command.Parameters.AddWithValue("@sNewDriverId", sNewDriverId);

                            connection.Open();

                            returnresult = command.ExecuteNonQuery();

                            connection.Close();
                            return returnresult;
                        }
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return returnresult;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return returnresult;
                }
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                return returnresult;
            }
        }

        public int UpdateIndividualPostalZoneMaster(DataSet oDTCompanyList, string sCompany, DataTable dtResult)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            int returnresult = 0;
            try
            {
                sFuncName = "UpdateIndividualPostalZoneMaster()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                if (oDTCompanyList != null && oDTCompanyList.Tables.Count > 0)
                {
                    oDTView = oDTCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        using (SqlConnection connection = new SqlConnection(oDTView[0]["U_ConnString"].ToString()))
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            foreach (DataRow dr in dtResult.Rows)
                            {
                                if (dr["NewDriverId"].ToString() != string.Empty && dr["DefaultDriverId"].ToString() != string.Empty && dr["NewDriverId"].ToString() != dr["DefaultDriverId"].ToString())
                                {
                                    command.CommandText = "Declare @NewDriverName varchar(max);set @NewDriverName = (select top 1 jobTitle from OHEM with (NOLOCK) where empID = @sNewDriverId);UPDATE [ADV_PostalZoneMaster] set DefaultDriverId = @sNewDriverId , DefaultDriverName = @NewDriverName where DefaultDriverId = @sOldDriverId and Id = @sId and IsNull(DefaultDriverDate,'') = '' AND IsMaster = 0 ";

                                    command.Parameters.AddWithValue("@sOldDriverId", dr["DefaultDriverId"].ToString());
                                    command.Parameters.AddWithValue("@sNewDriverId", dr["NewDriverId"].ToString());
                                    command.Parameters.AddWithValue("@sId", dr["Id"].ToString());

                                    connection.Open();
                                    command.ExecuteNonQuery();
                                    connection.Close();
                                    command.Parameters.Clear();
                                    returnresult = returnresult + 1;
                                }
                                else if (dr["DefaultDriverId"].ToString() == string.Empty && dr["NewDriverId"].ToString() != string.Empty)
                                {
                                    command.CommandText = "Declare @NewDriverName varchar(max);set @NewDriverName = (select top 1 jobTitle from OHEM with (NOLOCK) where empID = @sNewDriverId);UPDATE [ADV_PostalZoneMaster] set DefaultDriverId = @sNewDriverId , DefaultDriverName = @NewDriverName where Id = @sId and IsNull(DefaultDriverDate,'') = '' AND IsMaster = 0 ";

                                    command.Parameters.AddWithValue("@sNewDriverId", dr["NewDriverId"].ToString());
                                    command.Parameters.AddWithValue("@sId", dr["Id"].ToString());

                                    connection.Open();
                                    command.ExecuteNonQuery();
                                    connection.Close();
                                    command.Parameters.Clear();
                                    returnresult = returnresult + 1;
                                }

                            }
                            oLog.WriteToDebugLogFile("Result Count " + returnresult, sFuncName);
                            return returnresult;

                        }
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return returnresult;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return returnresult;
                }
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                return returnresult;
            }
        }

        #endregion

        #region Driver list by day
        public DataSet Get_DriverListbyDay(DataSet oDTCompanyList, string sCompany, string sDriverDate)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            string sProcName = string.Empty;
            DataView oDTView = new DataView();

            try
            {
                sFuncName = "Get_DriverListbyDay()";
                sProcName = "VS_SP005_Web_GetDriverListbyDay";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                oLog.WriteToDebugLogFile("Calling Run_StoredProcedure() " + sProcName, sFuncName);
                if (oDTCompanyList != null && oDTCompanyList.Tables.Count > 0)
                {
                    oDTView = oDTCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        oDataset = SqlHelper.ExecuteDataSet(oDTView[0]["U_ConnString"].ToString(), CommandType.StoredProcedure, sProcName,
                            Data.CreateParameter("@Company", sCompany), Data.CreateParameter("@DriverDate", sDriverDate));

                        oLog.WriteToDebugLogFile("Completed With SUCCESS ", sFuncName);
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return oDataset;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return oDataset;
                }
                return oDataset;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        public int UpdateBulkDriverListbyDay(DataSet oDTCompanyList, string sCompany, string sOldDriverId, string sNewDriverId, string sDriverDate)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            int returnresult = 0;
            try
            {
                sFuncName = "UpdateBulkDriverListbyDay()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                if (oDTCompanyList != null && oDTCompanyList.Tables.Count > 0)
                {
                    oDTView = oDTCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        using (SqlConnection connection = new SqlConnection(oDTView[0]["U_ConnString"].ToString()))
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            if (sOldDriverId != string.Empty)
                            {
                                command.CommandText = "Declare @NewDriverName varchar(max);set @NewDriverName = (select top 1 jobTitle from OHEM with (NOLOCK) where empID = @sNewDriverId);UPDATE [ADV_PostalZoneMaster] set DefaultDriverId = @sNewDriverId , DefaultDriverName = @NewDriverName where DefaultDriverId = @sOldDriverId and IsNull(DefaultDriverDate,'') = @sDriverDate AND IsMaster = 1 ";

                                command.Parameters.AddWithValue("@sOldDriverId", sOldDriverId);
                                command.Parameters.AddWithValue("@sNewDriverId", sNewDriverId);
                                command.Parameters.AddWithValue("@sDriverDate", sDriverDate);
                            }
                            else
                            {
                                command.CommandText = "Declare @NewDriverName varchar(max);set @NewDriverName = (select top 1 jobTitle from OHEM with (NOLOCK) where empID = @sNewDriverId);UPDATE [ADV_PostalZoneMaster] set DefaultDriverId = @sNewDriverId , DefaultDriverName = @NewDriverName where IsNull(DefaultDriverDate,'') = @sDriverDate AND IsMaster = 1 ";

                                command.Parameters.AddWithValue("@sNewDriverId", sNewDriverId);
                                command.Parameters.AddWithValue("@sDriverDate", sDriverDate);
                            }
                            connection.Open();

                            returnresult = command.ExecuteNonQuery();

                            connection.Close();
                            return returnresult;
                        }
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return returnresult;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return returnresult;
                }
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                return returnresult;
            }
        }

        public int UpdateIndividualDriverListbyDay(DataSet oDTCompanyList, string sCompany, DataTable dtResult)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            int returnresult = 0;
            try
            {
                sFuncName = "UpdateIndividualDriverListbyDay()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                if (oDTCompanyList != null && oDTCompanyList.Tables.Count > 0)
                {
                    oDTView = oDTCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        using (SqlConnection connection = new SqlConnection(oDTView[0]["U_ConnString"].ToString()))
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            foreach (DataRow dr in dtResult.Rows)
                            {

                                if (dr["NewDriverId"].ToString() != string.Empty && dr["DefaultDriverId"].ToString() != string.Empty && dr["NewDriverId"].ToString() != dr["DefaultDriverId"].ToString())
                                {
                                    //command.CommandText = "Declare @NewDriverName varchar(max);set @NewDriverName = (select top 1 jobTitle from OHEM with (NOLOCK) where empID = @sNewDriverId);UPDATE [ADV_PostalZoneMaster] set DefaultDriverId = @sNewDriverId , DefaultDriverName = @NewDriverName where DefaultDriverId = @sOldDriverId and Id = @sId and IsNull(DefaultDriverDate,'') = '' AND IsMaster = 0 ";
                                    command.CommandText = "Declare @NewDriverName varchar(max);set @NewDriverName = (select top 1 jobTitle from OHEM with (NOLOCK) where empID = @sNewDriverId);UPDATE [ADV_PostalZoneMaster] set DefaultDriverId = @sNewDriverId , DefaultDriverName = @NewDriverName where DefaultDriverId = @sOldDriverId and Id = @sId AND IsMaster = 1 ";

                                    command.Parameters.AddWithValue("@sOldDriverId", dr["DefaultDriverId"].ToString());
                                    command.Parameters.AddWithValue("@sNewDriverId", dr["NewDriverId"].ToString());
                                    command.Parameters.AddWithValue("@sId", dr["Id"].ToString());

                                    connection.Open();
                                    command.ExecuteNonQuery();
                                    connection.Close();
                                    command.Parameters.Clear();
                                    returnresult = returnresult + 1;
                                }
                                else if (dr["DefaultDriverId"].ToString() == string.Empty && dr["NewDriverId"].ToString() != string.Empty)
                                {
                                    //command.CommandText = "Declare @NewDriverName varchar(max);set @NewDriverName = (select top 1 jobTitle from OHEM with (NOLOCK) where empID = @sNewDriverId);UPDATE [ADV_PostalZoneMaster] set DefaultDriverId = @sNewDriverId , DefaultDriverName = @NewDriverName where Id = @sId and IsNull(DefaultDriverDate,'') = '' AND IsMaster = 0 ";
                                    command.CommandText = "Declare @NewDriverName varchar(max);set @NewDriverName = (select top 1 jobTitle from OHEM with (NOLOCK) where empID = @sNewDriverId);UPDATE [ADV_PostalZoneMaster] set DefaultDriverId = @sNewDriverId , DefaultDriverName = @NewDriverName where Id = @sId AND IsMaster = 1 ";

                                    command.Parameters.AddWithValue("@sNewDriverId", dr["NewDriverId"].ToString());
                                    command.Parameters.AddWithValue("@sId", dr["Id"].ToString());

                                    connection.Open();
                                    command.ExecuteNonQuery();
                                    connection.Close();
                                    command.Parameters.Clear();
                                    returnresult = returnresult + 1;
                                }
                            }
                            oLog.WriteToDebugLogFile("Result Count " + returnresult, sFuncName);

                            return returnresult;
                        }
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return returnresult;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return returnresult;
                }
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                return returnresult;
            }
        }
        #endregion

        #region DO Assignment

        public DataSet Get_Drivers_DOAssignment(DataSet oDTCompanyList, string sCompany)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            string sProcName = string.Empty;
            DataView oDTView = new DataView();

            try
            {
                sFuncName = "Get_Drivers_DOAssignment()";
                sProcName = "VS_SP006_Web_GetDriver_DOAssignment";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                oLog.WriteToDebugLogFile("Calling Run_StoredProcedure() " + sProcName, sFuncName);
                if (oDTCompanyList != null && oDTCompanyList.Tables.Count > 0)
                {
                    oDTView = oDTCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        oDataset = SqlHelper.ExecuteDataSet(oDTView[0]["U_ConnString"].ToString(), CommandType.StoredProcedure, sProcName,
                            Data.CreateParameter("@Company", sCompany));

                        oLog.WriteToDebugLogFile("Completed With SUCCESS ", sFuncName);
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return oDataset;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return oDataset;
                }
                return oDataset;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        public DataSet Get_NewDrivers_DOAssignment(DataSet oDTCompanyList, string sCompany)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            string sProcName = string.Empty;
            DataView oDTView = new DataView();

            try
            {
                sFuncName = "Get_NewDrivers_DOAssignment()";
                sProcName = "VS_SP007_Web_GetNewDriver_DOAssignment";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                oLog.WriteToDebugLogFile("Calling Run_StoredProcedure() " + sProcName, sFuncName);
                if (oDTCompanyList != null && oDTCompanyList.Tables.Count > 0)
                {
                    oDTView = oDTCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        oDataset = SqlHelper.ExecuteDataSet(oDTView[0]["U_ConnString"].ToString(), CommandType.StoredProcedure, sProcName,
                            Data.CreateParameter("@Company", sCompany));

                        oLog.WriteToDebugLogFile("Completed With SUCCESS ", sFuncName);
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return oDataset;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return oDataset;
                }
                return oDataset;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        public DataSet Get_PostalZoneandAssignedDO(DataSet oDTCompanyList, string sCompany, string sFromDate, string sToDate)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            string sProcName = string.Empty;
            DataView oDTView = new DataView();

            try
            {
                sFuncName = "Get_PostalZoneandAssignedDO()";
                sProcName = "VS_SP008_Web_GetPostalZoneandAssignedDO";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                oLog.WriteToDebugLogFile("Calling Run_StoredProcedure() " + sProcName, sFuncName);
                if (oDTCompanyList != null && oDTCompanyList.Tables.Count > 0)
                {
                    oDTView = oDTCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        oDataset = SqlHelper.ExecuteDataSet(oDTView[0]["U_ConnString"].ToString(), CommandType.StoredProcedure, sProcName,
                            Data.CreateParameter("@Company", sCompany), Data.CreateParameter("@FromDate", sFromDate), Data.CreateParameter("@ToDate", sToDate));

                        oLog.WriteToDebugLogFile("Completed With SUCCESS ", sFuncName);
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return oDataset;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return oDataset;
                }
                return oDataset;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        public int ReplaceDODrivers(DataSet oDTCompanyList, string sCompany, DataTable dtResult)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            string sDONumber = string.Empty;
            int returnresult = 0;
            try
            {
                sFuncName = "ReplaceDODrivers()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                if (oDTCompanyList != null && oDTCompanyList.Tables.Count > 0)
                {
                    oDTView = oDTCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        using (SqlConnection connection = new SqlConnection(oDTView[0]["U_ConnString"].ToString()))
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            foreach (DataRow dr in dtResult.Rows)
                            {
                                sDONumber = sDONumber + "'" + dr["DONumber"].ToString() + "',";
                            }
                            if (dtResult.Rows.Count > 0)
                            {
                                sDONumber = sDONumber.Remove(sDONumber.Length - 1);
                                command.CommandText = "Update ODLN SET U_OB_DriverName = '" + dtResult.Rows[0]["Driver"].ToString() + "' Where convert(varchar(max),DocNum) in (" + sDONumber + ")";
                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close();
                                returnresult = returnresult + 1;
                            }

                            oLog.WriteToDebugLogFile("Result Count " + returnresult, sFuncName);

                            return returnresult;
                        }
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return returnresult;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return returnresult;
                }
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                return returnresult;
            }
        }

        public DataSet DOSearch(DataSet oDTCompanyList, string sCompany, string sFromDate, string sToDate, string sStatus, string sDriverName, string sTimeofDelivery)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            string sProcName = string.Empty;
            DataView oDTView = new DataView();

            try
            {
                sFuncName = "DOSearch()";
                sProcName = "VS_SP009_Web_DOSearch";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                oLog.WriteToDebugLogFile("Calling Run_StoredProcedure() " + sProcName, sFuncName);
                if (oDTCompanyList != null && oDTCompanyList.Tables.Count > 0)
                {
                    oDTView = oDTCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        oDataset = SqlHelper.ExecuteDataSet(oDTView[0]["U_ConnString"].ToString(), CommandType.StoredProcedure, sProcName,
                            Data.CreateParameter("@Company", sCompany), Data.CreateParameter("@FromDate", sFromDate), Data.CreateParameter("@ToDate", sToDate),
                            Data.CreateParameter("@Status", sStatus), Data.CreateParameter("@DriverName", sDriverName), Data.CreateParameter("@TimeofDelivery", sTimeofDelivery));

                        oLog.WriteToDebugLogFile("Completed With SUCCESS ", sFuncName);
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return oDataset;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return oDataset;
                }
                return oDataset;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        public int UpdateDO(DataSet dsCompanyList, string sCompany, string sDocDate, string sDONumber, string sDriver, string sTimeofDelivery, DataTable dtItemArray)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            int returnresult = 0;
            try
            {
                sFuncName = "UpdateDO()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                if (dsCompanyList != null && dsCompanyList.Tables.Count > 0)
                {
                    oDTView = dsCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        using (SqlConnection connection = new SqlConnection(oDTView[0]["U_ConnString"].ToString()))
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "Update ODLN SET U_OB_DriverName = '" + sDriver + "',U_OB_DDT = '" + sTimeofDelivery + "' Where DocNum = '" + sDONumber + "'";
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                            foreach (DataRow dr in dtItemArray.Rows)
                            {
                                command.CommandText = "Update DLN1 SET U_OB_DeliveryStatus = '" + dr["StatusTemp"].ToString() + "', U_OB_LineRemarks = '" + dr["LineRemarks"].ToString() + "' Where ItemCode = '" + dr["ItemCode"].ToString() + "' and DocEntry in (Select DocEntry from ODLN where DocNum = " + sDONumber + ")";
                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close();
                                returnresult = returnresult + 1;
                            }
                            oLog.WriteToDebugLogFile("Result Count " + returnresult, sFuncName);

                            return returnresult;
                        }
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return returnresult;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return returnresult;
                }
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                return returnresult;
            }
        }

        public int UpdateEmailSentStatus(DataSet dsCompanyList, string sCompany, string sDONumber, DataTable dtItemArray)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            int returnresult = 0;
            try
            {
                sFuncName = "UpdateEmailSentStatus()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                if (dsCompanyList != null && dsCompanyList.Tables.Count > 0)
                {
                    oDTView = dsCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        using (SqlConnection connection = new SqlConnection(oDTView[0]["U_ConnString"].ToString()))
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            foreach (DataRow dr in dtItemArray.Rows)
                            {
                                command.CommandText = "Update DLN1 SET U_OB_EmailSentStatus = 'Y' Where ItemCode = '" + dr["ItemCode"].ToString() + "' and DocEntry in (Select DocEntry from ODLN where DocNum = " + sDONumber + ")";
                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close();
                                returnresult = returnresult + 1;
                            }
                            oLog.WriteToDebugLogFile("Result Count " + returnresult, sFuncName);

                            return returnresult;
                        }
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return returnresult;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return returnresult;
                }
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                return returnresult;
            }
        }

        public int Update_ReturnNoteUDF(DataSet dsCompanyList, string sCompany, string sDONumber, long sReturnNoteNo)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            int returnresult = 0;
            try
            {
                sFuncName = "Update_ReturnNoteUDF()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                if (dsCompanyList != null && dsCompanyList.Tables.Count > 0)
                {
                    oDTView = dsCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        using (SqlConnection connection = new SqlConnection(oDTView[0]["U_ConnString"].ToString()))
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "Update ODLN SET U_OB_ReturnNoteNo = '" + sReturnNoteNo + "' Where DocNum = " + sDONumber + "";
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                            returnresult = returnresult + 1;

                            return returnresult;
                        }
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return returnresult;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return returnresult;
                }
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                return returnresult;
            }
        }

        public int Update_PrintUDF(DataSet dsCompanyList, string sCompany, string sDONumber)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            int returnresult = 0;
            try
            {
                sFuncName = "Update_PrintUDF()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                if (dsCompanyList != null && dsCompanyList.Tables.Count > 0)
                {
                    oDTView = dsCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        using (SqlConnection connection = new SqlConnection(oDTView[0]["U_ConnString"].ToString()))
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "Update ODLN SET U_OB_Approved = 'Y' Where DocNum = " + sDONumber + "";
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                            returnresult = returnresult + 1;

                            return returnresult;
                        }
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return returnresult;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return returnresult;
                }
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                return returnresult;
            }
        }

        public long CreateDOReturn(string sDocEntry, int sServiceCallId, SAPbobsCOM.Company p_oCompany, ref string sErrDesc)
        {
            long RTN_SUCCESS = 0;
            long RTN_ERROR = 1;
            long functionReturnValue = 0;
            string sFuncName = string.Empty;
            SAPbobsCOM.Documents oDO = default(SAPbobsCOM.Documents);
            SAPbobsCOM.Documents oReturn = default(SAPbobsCOM.Documents);
            long lRetCode = 0;
            int iTemp = 0;
            DataSet oDS = new DataSet();
            SAPbobsCOM.ServiceCalls oServiceCall = default(SAPbobsCOM.ServiceCalls);
            int i = 0;
            int iDONum = 0;
            int iCnt = 0;
            string sSAPReturnNo = string.Empty;
            int iSVCNo = 0;
            int iLineStatusCheck = 0;
            bool bCheckStatus = false;
            int iReturnNoteDocNO = 0;

            try
            {
                sFuncName = "CreateDeliveryOrder()";
                oLog.WriteToDebugLogFile("Starting Function", sFuncName);


                //oLog.WriteToDebugLogFile("Calling ConnectToTargetCompany()", sFuncName);
                //if (ConnectToTargetCompany(p_oCompany, sErrDesc) != RTN_SUCCESS)
                //    throw new ArgumentException(sErrDesc);;
                oLog.WriteToDebugLogFile("Before Initializing Delivery Notes Buisness Object", sFuncName);
                oDO = p_oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDeliveryNotes);
                oLog.WriteToDebugLogFile("Before Initializing Returns Buisness Object", sFuncName);
                oReturn = p_oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oReturns);

                //iDONum = 65073;
                iDONum = Convert.ToInt32(sDocEntry);
                //get the DO DocEntry based on DO
                //iSVCNo = 103673;
                iSVCNo = sServiceCallId;
                //get the Service call

                oLog.WriteToDebugLogFile("Before Initializing Service call Buisness Object", sFuncName);
                oServiceCall = p_oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oServiceCalls);

                oLog.WriteToDebugLogFile("Before Checking the DO using Doc Entry", sFuncName);
                if (oDO.GetByKey(iDONum) == true)
                {
                    oLog.WriteToDebugLogFile("Before assigning the values to Return Document", sFuncName);
                    oReturn.DocDate = DateTime.Now;
                    oReturn.CardCode = oDO.CardCode;
                    oReturn.Comments = oDO.Comments;
                    oReturn.UserFields.Fields.Item("U_OB_Sup_Type").Value = oDO.UserFields.Fields.Item("U_OB_Sup_Type").Value;
                    oReturn.UserFields.Fields.Item("U_OB_Reference").Value = oDO.UserFields.Fields.Item("U_OB_Reference").Value;
                    oReturn.UserFields.Fields.Item("U_OB_DDT").Value = oDO.UserFields.Fields.Item("U_OB_DDT").Value;

                    //Line Details

                    for (i = 0; i <= oDO.Lines.Count - 1; i++)
                    {
                        oDO.Lines.SetCurrentLine(i);

                        if (oDO.Lines.UserFields.Fields.Item("U_OB_DeliveryStatus").Value.ToString().Contains("Cancelled"))
                        {
                            iLineStatusCheck = iLineStatusCheck + 1;
                            iCnt += 1;
                            if (iCnt > 1)
                            {
                                oReturn.Lines.Add();
                            }

                            oReturn.Lines.SetCurrentLine(iCnt - 1);

                            oReturn.Lines.ItemCode = oDO.Lines.ItemCode;
                            oReturn.Lines.Quantity = oDO.Lines.Quantity;
                            oReturn.Lines.Price = oDO.Lines.Price;
                            oReturn.Lines.UnitPrice = oDO.Lines.UnitPrice;

                            oReturn.Lines.SerialNumbers.InternalSerialNumber = oDO.Lines.SerialNumbers.InternalSerialNumber;
                            oReturn.Lines.BatchNumbers.BatchNumber = oDO.Lines.BatchNumbers.BatchNumber;

                            oReturn.Lines.BaseEntry = oDO.DocEntry;
                            oReturn.Lines.BaseLine = oDO.Lines.LineNum;
                            oReturn.Lines.BaseType = 15;

                            oReturn.Lines.WarehouseCode = oDO.Lines.WarehouseCode;
                            oReturn.Lines.COGSCostingCode = oDO.Lines.COGSCostingCode;

                            oReturn.Lines.UserFields.Fields.Item("U_OB_IType").Value = oDO.Lines.UserFields.Fields.Item("U_OB_IType").Value;
                            oReturn.Lines.UserFields.Fields.Item("U_OB_EquipItemCode").Value = oDO.Lines.UserFields.Fields.Item("U_OB_EquipItemCode").Value;
                            oReturn.Lines.UserFields.Fields.Item("U_OB_EquipItemName").Value = oDO.Lines.UserFields.Fields.Item("U_OB_EquipItemName").Value;
                            oReturn.Lines.UserFields.Fields.Item("U_OB_DODate").Value = oDO.Lines.UserFields.Fields.Item("U_OB_DODate").Value;
                            oReturn.Lines.UserFields.Fields.Item("U_OB_ContractNo").Value = oDO.Lines.UserFields.Fields.Item("U_OB_ContractNo").Value;
                            oReturn.Lines.UserFields.Fields.Item("U_OB_SC").Value = oDO.Lines.UserFields.Fields.Item("U_OB_SC").Value;

                            oReturn.Lines.UserFields.Fields.Item("U_OB_Dept").Value = oDO.Lines.UserFields.Fields.Item("U_OB_Dept").Value;
                            oReturn.Lines.UserFields.Fields.Item("U_OB_Attn").Value = oDO.Lines.UserFields.Fields.Item("U_OB_Attn").Value;
                            oReturn.Lines.UserFields.Fields.Item("U_OB_Address").Value = oDO.Lines.UserFields.Fields.Item("U_OB_Address").Value;
                            oReturn.Lines.UserFields.Fields.Item("U_OB_CECID").Value = oDO.Lines.UserFields.Fields.Item("U_OB_CECID").Value;
                        }
                    }

                    oLog.WriteToDebugLogFile("After Assigning the Return Document Values", sFuncName);

                    if (iLineStatusCheck != 0)
                    {
                        oLog.WriteToDebugLogFile("Calling oReturn.Add()", sFuncName);

                        lRetCode = oReturn.Add();
                        if (lRetCode != 0)
                        {
                            p_oCompany.GetLastError(out iTemp, out sErrDesc);
                            throw new ArgumentException(sErrDesc);
                        }

                        p_oCompany.GetNewObjectCode(out sSAPReturnNo);

                        
                        SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)p_oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        string sQuery = "SELECT DocNum from ORDN Where DocEntry = " + sSAPReturnNo + "";
                        oLog.WriteToDebugLogFile(sQuery, sFuncName);

                        oRS.DoQuery(sQuery);

                        oLog.WriteToDebugLogFile("DocNum :" + oRS.Fields.Item("DocNum").Value, sFuncName);
                        iReturnNoteDocNO = oRS.Fields.Item("DocNum").Value;
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oRS); // this line is to release the Record set

                        if (oServiceCall.GetByKey(iSVCNo) == true)
                        {
                            var sCallStatus = oServiceCall.Status;
                            if (sCallStatus == -1) // -1 means Closed
                            {
                                oServiceCall.Status = -3; // -3 means Open

                                if (oServiceCall.Update() != 0)
                                {
                                    bCheckStatus = false; // THis variable is true means, the service call is already closed and we manually opened for update
                                    throw new ArgumentException(p_oCompany.GetLastErrorDescription());
                                }
                                else
                                {
                                    bCheckStatus = true; // THis variable is true means, the service call is already closed and we manually opened for update
                                }
                            }
                            int iCount = 0;
                            iCount = oServiceCall.Expenses.Count;
                            oServiceCall.Expenses.Add();
                            oServiceCall.Expenses.SetCurrentLine(iCount);
                            oServiceCall.Expenses.DocumentType = SAPbobsCOM.BoSvcEpxDocTypes.edt_Return;
                            oServiceCall.Expenses.DocEntry = Convert.ToInt32(sSAPReturnNo);
                            if (bCheckStatus == true)
                            {
                                oServiceCall.Status = -1; // -1 means Closed
                                bCheckStatus = false; // THis variable is true means, the service call is already closed and we manually opened for update
                            }
                        }

                        oLog.WriteToDebugLogFile("Calling oServiceCall.Update()", sFuncName);
                        lRetCode = oServiceCall.Update();
                        if (lRetCode != 0)
                        {
                            p_oCompany.GetLastError(out iTemp, out sErrDesc);
                            throw new ArgumentException(sErrDesc);
                        }

                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("Cannot add the return Document.", sFuncName);
                    }
                }

                oLog.WriteToDebugLogFile("Completed with SUCCESS", sFuncName);
                functionReturnValue = iReturnNoteDocNO;

            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message;
                oLog.WriteToErrorLogFile(ex.Message, sFuncName);

                oLog.WriteToDebugLogFile("Completed with ERROR", sFuncName);
                functionReturnValue = RTN_ERROR;
            }
            finally
            {
                oDO = null;
            }
            return functionReturnValue;

        }

        #endregion

        #region CompanyConnection
        public SAPbobsCOM.Company ConnectToTargetCompany(string sCompanyDB)
        {
            string sFuncName = string.Empty;
            string sReturnValue = string.Empty;
            DataSet oDTCompanyList = new DataSet();
            DataSet oDSResult = new DataSet();
            string sConnString = string.Empty;
            DataView oDTView = new DataView();

            try
            {
                sFuncName = "ConnectToTargetCompany()";

                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);


                if (oDICompany != null)
                {
                    if (oDICompany.CompanyDB == sCompanyDB)
                    {
                        oLog.WriteToDebugLogFile("ODICompany Name " + oDICompany.CompanyDB, sFuncName);
                        oLog.WriteToDebugLogFile("SCompanyDB " + sCompanyDB, sFuncName);
                        return oDICompany;
                    }

                }

                oLog.WriteToDebugLogFile("Calling Get_Company_Details() ", sFuncName);
                oDTCompanyList = Get_CompanyList();

                oLog.WriteToDebugLogFile("Calling Filter Based on Company DB() ", sFuncName);
                oDTView = oDTCompanyList.Tables[0].DefaultView;
                oDTView.RowFilter = "U_DBName= '" + sCompanyDB + "'";

                oLog.WriteToDebugLogFile("Calling ConnectToTargetCompany() ", sFuncName);

                sConnString = oDTView[0]["U_ConnString"].ToString();

                oDICompany = ConnectToTargetCompany(oDICompany, oDTView[0]["U_SAPUserName"].ToString(), oDTView[0]["U_SAPPassword"].ToString()
                                   , oDTView[0]["U_DBName"].ToString(), oDTView[0]["U_Server"].ToString(), oDTView[0]["U_LicenseServer"].ToString()
                                   , oDTView[0]["U_DBUserName"].ToString(), oDTView[0]["U_DBPassword"].ToString(), sErrDesc);

                oLog.WriteToDebugLogFile("Completed With SUCCESS  ", sFuncName);

                return oDICompany;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                throw Ex;
            }

        }

        public SAPbobsCOM.Company ConnectToTargetCompany(SAPbobsCOM.Company oCompany, string sUserName, string sPassword, string sDBName,
                                                        string sServer, string sLicServerName, string sDBUserName
                                                       , string sDBPassword, string sErrDesc)
        {
            string sFuncName = string.Empty;
            long lRetCode;

            try
            {
                sFuncName = "ConnectToTargetCompany()";

                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                if (oCompany != null)
                {
                    oLog.WriteToDebugLogFile("Disconnecting the Company object - Company Name " + oCompany.CompanyName, sFuncName);
                    oCompany.Disconnect();
                }
                oLog.WriteToDebugLogFile("Before initializing ", sFuncName);

                oCompany = new SAPbobsCOM.Company();
                oLog.WriteToDebugLogFile("After Initializing Company Connection ", sFuncName);
                oCompany.Server = sServer;
                oCompany.LicenseServer = sLicServerName;
                oCompany.DbUserName = sDBUserName;
                oCompany.DbPassword = sDBPassword;
                oCompany.language = SAPbobsCOM.BoSuppLangs.ln_English;
                oCompany.UseTrusted = false;
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008;


                oCompany.CompanyDB = sDBName;// sDataBaseName;
                oCompany.UserName = sUserName;
                oCompany.Password = sPassword;

                oLog.WriteToDebugLogFile("Connecting the Database...", sFuncName);

                lRetCode = oCompany.Connect();

                if (lRetCode != 0)
                {
                    throw new ArgumentException(oCompany.GetLastErrorDescription());
                }
                else
                {
                    oLog.WriteToDebugLogFile("Company Connection Established", sFuncName);
                    oLog.WriteToDebugLogFile("Completed With SUCCESS", sFuncName);
                    return oCompany;
                }
            }
            catch (Exception Ex)
            {

                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }
        #endregion

        #region Other DO & Collection Note

        public DataSet SearchOtherDOandCN(DataSet oDTCompanyList, string sCompany, string sFromDate, string sToDate, string sCustName, string sDocNum, string sDriverInCharge, string sStatus, string sType)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            string sProcName = string.Empty;
            DataView oDTView = new DataView();

            try
            {
                sFuncName = "SearchOtherDOandCN()";
                sProcName = "VS_SP012_Web_SearchOtherDOandCN";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                oLog.WriteToDebugLogFile("Calling Run_StoredProcedure() " + sProcName, sFuncName);
                if (oDTCompanyList != null && oDTCompanyList.Tables.Count > 0)
                {
                    oDTView = oDTCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        oDataset = SqlHelper.ExecuteDataSet(oDTView[0]["U_ConnString"].ToString(), CommandType.StoredProcedure, sProcName,
                            Data.CreateParameter("@FromDate", sFromDate),Data.CreateParameter("@ToDate", sToDate), Data.CreateParameter("@CustName", sCustName),
                            Data.CreateParameter("@DocNum", sDocNum), Data.CreateParameter("@DriverInCharge", sDriverInCharge),
                            Data.CreateParameter("@Status", sStatus), Data.CreateParameter("@Type", sType));

                        oLog.WriteToDebugLogFile("Completed With SUCCESS ", sFuncName);
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return oDataset;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return oDataset;
                }
                return oDataset;
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                throw Ex;
            }
        }

        public int UpdateOtherDOandCN(DataSet dsCompanyList, string sCompany, string sDocEntry, string sDocNum, string sDriverInCharge1, string sDriverInCharge2, string sDriverInCharge3, string sDeliveryTime, string sHeaderTable, DataTable dtItemArray)
        {
            DataSet oDataset = new DataSet();
            string sFuncName = string.Empty;
            DataView oDTView = new DataView();
            int returnresult = 0;
            try
            {
                sFuncName = "UpdateOtherDOandCN()";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);

                if (dsCompanyList != null && dsCompanyList.Tables.Count > 0)
                {
                    oDTView = dsCompanyList.Tables[0].DefaultView;

                    oDTView.RowFilter = "U_DBName= '" + sCompany + "'";
                    if (oDTView != null && oDTView.Count > 0)
                    {
                        using (SqlConnection connection = new SqlConnection(oDTView[0]["U_ConnString"].ToString()))
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "Update " + sHeaderTable + " SET U_OB_DriverName = '" + sDriverInCharge1 + "',U_OB_DriverName1 = '" + sDriverInCharge2 + "',U_OB_DriverName2 = '" + sDriverInCharge3 + "',U_OB_DeliveryTime = '" + sDeliveryTime + "' Where DocEntry = '" + sDocEntry + "'";
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                            foreach (DataRow dr in dtItemArray.Rows)
                            {
                                command.CommandText = "Update " + dr["LineTable"].ToString() + " SET U_OB_DeliveryStatus = '" + dr["DeliveryStatus"].ToString() + "', U_OB_LineRemarks = '" + dr["LineRemarks"].ToString() + "' Where ItemCode = '" + dr["ItemCode"].ToString() + "' and DocEntry in (Select DocEntry from " + sHeaderTable + " where DocEntry = " + sDocEntry + ")";
                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close();
                                returnresult = returnresult + 1;
                            }
                            oLog.WriteToDebugLogFile("Result Count " + returnresult, sFuncName);

                            return returnresult;
                        }
                    }
                    else
                    {
                        oLog.WriteToDebugLogFile("No Data in OUSR table for the selected Company", sFuncName);
                        return returnresult;
                    }
                }
                else
                {
                    oLog.WriteToDebugLogFile("There is No Company List in the Holding Company ", sFuncName);
                    return returnresult;
                }
            }
            catch (Exception Ex)
            {
                sErrDesc = Ex.Message.ToString();
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToDebugLogFile("Completed With ERROR  ", sFuncName);
                return returnresult;
            }
        }

        #endregion

    }
}