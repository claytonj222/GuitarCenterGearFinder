using GuitarCenterGearFinder.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Security;

namespace GuitarCenterGearFinder.Classes
{
    public class EmailErrorHandler : IErrorHandler
    {
        public string UserName { get; set; }
        public SmtpClient EmailServer { get; set; }


        public EmailErrorHandler(string userName, SecureString password, int port, string host)
        {
            UserName = userName;
            EmailServer = new SmtpClient();

            EmailServer.Port = port;
            EmailServer.Host = host;
            EmailServer.EnableSsl = true;
            EmailServer.UseDefaultCredentials = false;
            EmailServer.Credentials = new NetworkCredential(userName, password);
            EmailServer.DeliveryMethod = SmtpDeliveryMethod.Network;
        }

        public void SendError(Exception ex, string destinationEmail)
        {
            try
            {
                if (string.IsNullOrEmpty(destinationEmail))
                {
                    return;
                }

                INotificationData data = PrepareData(ex);

                MailMessage errorMail = new MailMessage();
                errorMail.From = new MailAddress(UserName);
                errorMail.To.Add(new MailAddress(destinationEmail));
                errorMail.Body = data.Body;
                errorMail.Subject = data.Subject;

                EmailServer.Send(errorMail);
            }
            catch (Exception e)
            {
                Tracer.PrintDetailedException(e);
            }
        }

        public INotificationData PrepareData(Exception ex)
        {
            return new EmailNotificationData()
            {
                Subject = string.Format("An exception has occurred with {0}", ex.Source),
                Body = Tracer.GetDetailedException(ex)
            };
        }
    }
}
