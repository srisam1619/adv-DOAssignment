using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADV_WebService_V001;
using System.Net.Mail;
using System.Configuration;
using System.Data;

namespace ADV_BLL_V001
{
    public class TestEmail_Backup
    {
        clsLog oLog = new clsLog();
        long RTN_SUCCESS = 0;
        long RTN_ERROR = 1;

        string sFromMailId = ConfigurationManager.AppSettings["FromMailId"];
        //string sFromMailIdPassword = ConfigurationManager.AppSettings["FromMailIdPassword"];
        string sSMTPHost = ConfigurationManager.AppSettings["SMTPHost"];
        int iSMTPPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
        string sSMTPUser = ConfigurationManager.AppSettings["SMTPUser"];
        string sSMTPPassword = ConfigurationManager.AppSettings["SMTPPassword"];
        int iSMTPConnTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPConnTimeout"]);
        string sCCEmailIds = ConfigurationManager.AppSettings["CCEmailId"];

        public long SendEmail(string sBody, string sEmailTo, string sSubject, ref string sErrDesc)
        {
            long functionReturnValue = 0;

            string sFuncName = "SendEmail";

            try
            {
                oLog.WriteToDebugLogFile("Sarting function", sFuncName);
                oLog.WriteToDebugLogFile("Setting SMTP properties", sFuncName);
                SmtpClient smtpClient = new SmtpClient(sSMTPHost, iSMTPPort);

                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(sSMTPUser, sSMTPPassword);

                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                // smtpClient.EnableSsl = True
                smtpClient.EnableSsl = false;

                oLog.WriteToDebugLogFile("Calling Function CreateDefaultMailMessage()", sFuncName);

                // Split Email string based on ";"

                string[] sSendTo = sEmailTo.Split(';');
                string[] sCC = sCCEmailIds.Split(';');

                MailMessage message = CreateDefaultMailMessage(sFromMailId, sSendTo, sCC, sSubject, sBody, ref sErrDesc);
                object userState = message;

                oLog.WriteToDebugLogFile("Sending Email Message", sFuncName);

                oLog.WriteToDebugLogFile("Sending Email Messages to : " + sEmailTo, sFuncName);

                smtpClient.Send(message);

                functionReturnValue = RTN_SUCCESS;

                oLog.WriteToDebugLogFile("Function completed with Success", sFuncName);

            }
            catch (Exception ex)
            {
                functionReturnValue = RTN_ERROR;
                sErrDesc = ex.Message;

                oLog.WriteToDebugLogFile("Function completed with Error", sFuncName);
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                oLog.WriteToErrorLogFile("Failed sending email to : " + " " + sEmailTo, sFuncName);

            }
            finally
            {
            }
            return functionReturnValue;

        }

        private object smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            string smessage = string.Empty;
            if (((e.Error != null)))
            {
                smessage = e.Error.Message;
            }
            return smessage;
        }

        private MailMessage CreateDefaultMailMessage(string MailFrom, string[] MailTo, string[] CC, string subject, string body, ref string sErrDesc)
        {
            MailMessage functionReturnValue = default(MailMessage);

            MailMessage message = new MailMessage();
            string sUploadFile = string.Empty;
            string sFuncName = "CreateDefaultMailMessage";
            string sEmailAddress = string.Empty;
            string sAttachements = string.Empty;
            string sFileName = string.Empty;

            try
            {

                oLog.WriteToDebugLogFile("Sarting function", sFuncName);


                oLog.WriteToDebugLogFile("Assigning Email Properties..", sFuncName);

                message.From = new MailAddress(MailFrom);

                foreach (string sEmailAddress_loopVariable in MailTo)
                {
                    sEmailAddress = sEmailAddress_loopVariable;
                    message.To.Add(new MailAddress(sEmailAddress));

                    oLog.WriteToDebugLogFile("Adding From Email Address" + ":  " + sEmailAddress, sFuncName);
                }

                //if (bSendEmailVerification == false)
                foreach (string sCCAddress_loopVariable in CC)
                {
                    if (sCCAddress_loopVariable.ToString() != string.Empty)
                    {
                        message.CC.Add(new MailAddress(sCCAddress_loopVariable));
                        oLog.WriteToDebugLogFile("Adding CC Email Address" + ":  " + sCCAddress_loopVariable, sFuncName);
                    }
                }

                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.Subject = subject;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Body = body;
                message.IsBodyHtml = true;

                return message;

            }
            catch (Exception ex)
            {
                sErrDesc = ex.Message;

                oLog.WriteToDebugLogFile("Function completed with Error", sFuncName);
                oLog.WriteToErrorLogFile(sErrDesc, sFuncName);
                functionReturnValue = null;

            }
            finally
            {
            }
            return functionReturnValue;

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
    }
}
