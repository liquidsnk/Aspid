#region License
#endregion

using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;

using Aspid.Core.Properties;

namespace Aspid.Core
{
    public static class CrashReporting
    {
        static readonly ILogger logger = Logging.GetLogger("CrashReporting");

        static MailMessage emailBeingSent;
        static object emailEnabledLock = new object();
        
        public static bool SupportEmailIsHtml
        {
            get { return Settings.Default.SupportMailFormatIsHtml; }
        }

        public static void SendSupportEmail(string subject, string body)
        {
            using (var mail = GetSupportEmail(subject, body))
            {
                SendSupportEmail(mail);
            }
        }

        public static MailMessage GetSupportEmail(string subject, string body)
        {
            return new MailMessage
            {
               From = new MailAddress(Settings.Default.SupportSendAddress, Settings.Default.SupportSendFriendlyName),
               Subject = subject,
               Body = body,
               IsBodyHtml = SupportEmailIsHtml
            };
        }

        public static void SendSupportEmail(MailMessage mail)
        {
            SendSupportEmail(mail, true, true);
        }

        public static void SendSupportEmail(MailMessage mail, bool async, bool addSupportRecipients)
        {
            logger.LogDebug("Sending support email.");

            try
            {
                if (addSupportRecipients)
                {
                    //Send to all support recipients
                    foreach (var recipient in Settings.Default.SupportRecipients.Split(',').Select(x => x.Trim()))
                    {
                        if (string.IsNullOrEmpty(recipient)) return;
                        mail.To.Add(recipient);
                    }
                }

                var smtpClient = new SmtpClient();

                lock (emailEnabledLock)
                {
                    if (emailBeingSent == null)
                    {
                        emailBeingSent = mail;

                        if (async)
                        {
                            smtpClient.SendCompleted += smtpClient_SendCompleted;
                            smtpClient.SendAsync(mail, null);
                        }
                        else
                        {
                            smtpClient.Send(mail);
                            emailBeingSent = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSupportEmailError(ex);
            }
        }

        static void smtpClient_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            emailBeingSent = null;

            if (e.Error != null)
            {
                LogSupportEmailError(e.Error);
            }
        }

        private static void LogSupportEmailError(Exception ex)
        {
            logger.LogError("Error while trying to send support email");
            logger.LogException(ex);
        }
    }
}
