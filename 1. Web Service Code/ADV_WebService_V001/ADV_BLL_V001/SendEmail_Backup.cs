using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net.Mail;
using ADV_WebService_V001;
using System.Configuration;
using System.Net;

namespace ADV_BLL_V001
{
    public class SendEmail_Backup
    {
        #region Objects

        MailMessage myMailMessage;
        string sFromMailId = ConfigurationManager.AppSettings["FromMailId"];
        string sFromMailIdPassword = ConfigurationManager.AppSettings["FromMailIdPassword"];
        string sSMTPHost = ConfigurationManager.AppSettings["SMTPHost"];
        int iSMTPPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
        int iSMTPConnTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPConnTimeout"]);
        string sCCEmailIds = ConfigurationManager.AppSettings["CCEmailId"];

        #endregion

        #region Methods

        public string SendAutomatedEmail(DataTable dt, string sEmailId, string ServiceCallId, string DONumber)
        {
            string sstrSent = "";
            string sFuncName = string.Empty;
            string sErrorDesc = string.Empty;
            clsLog oLog = new clsLog();
            try
            {
                sFuncName = "SendAutomatedEmail";
                oLog.WriteToDebugLogFile("Starting Function ", sFuncName);
                Console.WriteLine("Starting Function " + sFuncName);
                if (dt != null && dt.Rows.Count > 0)
                {
                    oLog.WriteToDebugLogFile("Composing the content for Email Id '" + sEmailId + "'", sFuncName);
                    myMailMessage = new MailMessage();

                    myMailMessage.IsBodyHtml = true;
                    myMailMessage.Body = ComposeBody(dt, DONumber);

                    //myMailMessage.CC.Add(new MailAddress(sCCEmailIds.ToString()));

                    myMailMessage.From = new MailAddress(sFromMailId);
                    myMailMessage.To.Add(new MailAddress(sEmailId));
                    myMailMessage.Subject = "Cancellation - " + ServiceCallId + " - " + DONumber + ".";

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = sSMTPHost;
                    smtp.Port = iSMTPPort;
                    smtp.Timeout = iSMTPConnTimeout;
                    smtp.Credentials = new NetworkCredential(sFromMailId, sFromMailIdPassword);
                    smtp.EnableSsl = true;

                    oLog.WriteToDebugLogFile("Before sending Mail to '" + sEmailId + "'", sFuncName);
                    smtp.Send(myMailMessage);
                    oLog.WriteToDebugLogFile("After sending Mail to '" + sEmailId + "'", sFuncName);
                    oLog.WriteToDebugLogFile("Email Sent Successfully...", sFuncName);
                    sstrSent = "SUCCESS";
                }
            }
            catch (Exception ex)
            {
                sstrSent = (ex.InnerException).InnerException.ToString();
                oLog.WriteToErrorLogFile(sstrSent, sFuncName);
                oLog.WriteToErrorLogFile("Completed with error ", sFuncName);
                return sstrSent;
            }
            return sstrSent;
        }

        public string ComposeBody(DataTable dtBody, string sDONumber)
        {
            string sBodyDetail = string.Empty;
            string sBodyDetail1 = string.Empty;
            string sTableFormat = string.Empty;
            foreach (DataRow item in dtBody.Rows)
            {
                sBodyDetail = "<tr><td> &nbsp;" + sDONumber + "</td><td>&nbsp;" + item["ReturnNoteNo"] + "</td><td>&nbsp;" + item["ItemCode"].ToString() + " </td> " +
                    " <td>&nbsp;" + item["ItemName"].ToString() + " </td><td>&nbsp;" + item["Quantity"].ToString() + " </td><td>&nbsp;" + item["StatusTemp"].ToString() + " </td> " +
                    " <td>&nbsp;" + item["LineRemarks"].ToString() + " </td></tr>";
                sBodyDetail1 = sBodyDetail1 + sBodyDetail;
            }
            sTableFormat = "<table border = '1' cellspacing = 0 cellpadding = 0 style='font-size:10.0pt;font-family:Arial;width: 85%;'> " +
                                "<tr><td><strong style='color: blue; background-color: transparent;'>&nbsp;DO Number&nbsp;</strong></td> " +
                                "<td><strong style='color: blue; background-color: transparent;'>&nbsp;Return Note No. &nbsp; </strong></td> " +
                                "<td><strong style='color: blue; background-color: transparent;'>&nbsp;Item &nbsp;</strong></td> " +
                                "<td><strong style='color: blue; background-color: transparent;'>&nbsp;Item Name &nbsp;</strong></td> " +
                                "<td><strong style='color: blue; background-color: transparent;'>&nbsp;Cancelled Qty &nbsp;</strong></td> " +
                                "<td><strong style='color: blue; background-color: transparent;'>&nbsp;Reason For Cancellation &nbsp;</strong></td> " +
                                "<td><strong style='color: blue; background-color: transparent;'>&nbsp;Remarks &nbsp;</strong></td></tr> " +
                                sBodyDetail1 + " </table> ";
            string stextBody = "<p style='font-size:10.0pt;font-family:Arial;'>Dear All,<br /><br /> Please take note that the following item has been cancelled due to the following reasons. " +
                                ".<br/><br /> " + sTableFormat + "<br/> Please put back into the stock and double check on SAP system. <br/> Thank you." +
                                " <br/><br/> *** [This e-mail is automatically generated from the system and requires no signature.] ***</p>";

            return stextBody;
        }

        #endregion
    }
}
