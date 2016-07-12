using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Diagnostics;

namespace NetroMedia.SharedFunctions
{
    public class Emailer
    {
         
        #region Fields
        AppConfig _appConfig = ConfigManager.LoadFromFile();
        EventLog _eventLog;
        #endregion

        public void sendEmail(ServiceToParse service, int InsertedRecords, string message)
        {
            sendEmail_(service, InsertedRecords, message, _appConfig.public_ip_this_server);
        }

        public void sendEmail(ServiceToParse service, int InsertedRecords, string message, string serverIP)
        {
            sendEmail_(service, InsertedRecords, message, serverIP);
        }

        public void sendEmail_(ServiceToParse service, int InsertedRecords, string message, string serverIP)
        {
            _eventLog = new EventLog("Application");

            try
            {

                // create the email message
                string[] toAd = _appConfig.email_to.Split(';');
                MailMessage emailMessage = new MailMessage();
                emailMessage.From = new MailAddress(_appConfig.email_from);
                foreach (string toAddress in toAd)
                {
                    emailMessage.To.Add(new MailAddress(toAddress));
                    //   _appConfig.email_to  
                }

                emailMessage.Subject = service.ToString() + " have a problem  " + DateTime.Now.ToString()  ;
                emailMessage.Body = "Server: Internal name: " + _appConfig.public_ip_this_server  + " \n " +  message;
                emailMessage.IsBodyHtml = true;

                // create smtp client at mail server location
                SmtpClient client = new SmtpClient(_appConfig.email_smtp);
                client.Port = 587;
                client.EnableSsl = true;

                client.Credentials = new NetworkCredential(_appConfig.email_user, _appConfig.email_pwd);

                // send message
                client.Send(emailMessage);


            }
            catch //( Exception ex )
            {
               // _eventLog.WriteEntry("email send error " + ex.Message, EventLogEntryType.Error);
            }

        }

    }
}
