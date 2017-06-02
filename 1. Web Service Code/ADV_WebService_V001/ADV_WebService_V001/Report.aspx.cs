using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace ADV_WebService_V001
{
    public partial class Report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string connection = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();
            string[] dbInfo = connection.Split(';');
            string strServer = dbInfo[0].Split('=')[1];//ConfigurationSettings.AppSettings["SQLserver"].ToString();
            string strDBName = dbInfo[1].Split('=')[1];//ConfigurationSettings.AppSettings["SQLdatabaseName"].ToString();
            string strUID = dbInfo[2].Split('=')[1];//ConfigurationSettings.AppSettings["SQLUserName"].ToString();
            string strPassword = dbInfo[3].Split('=')[1];//ConfigurationSettings.AppSettings["SQLPassword"].ToString();

            ReportDocument myReportDocument;
            myReportDocument = new ReportDocument();
            myReportDocument.Load(Server.MapPath("~/Reports/") + "VS_SP010_Web_RPT_DOCollectionNoteMain.rpt");
            myReportDocument.SetDatabaseLogon(strUID, strPassword, strServer, strDBName);

            string sDocKey = Convert.ToString(Request.QueryString["key"]);
            CrystalDecisions.Shared.ParameterValues pval1 = new ParameterValues();

            ParameterDiscreteValue pdisval1 = new ParameterDiscreteValue();

            pdisval1.Value = sDocKey;
            pval1.Add(pdisval1);

            myReportDocument.DataDefinition.ParameterFields["@DocNum"].ApplyCurrentValues(pval1);

            CrystalReportViewer1.ReportSource = myReportDocument;
        }
    }
}