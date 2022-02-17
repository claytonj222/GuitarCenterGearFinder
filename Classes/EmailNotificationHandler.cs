using GuitarCenterGearFinder.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Security;

namespace GuitarCenterGearFinder.Classes
{
    public class EmailNotificationHandler : INotificationHandler
    {
        public string ExistingItemNumbersPath { get; set; }
        public SmtpClient EmailServer { get; set; }
        public string UserName { get; set; }

        public EmailNotificationHandler(string filePath, string userName, SecureString password, int port, string host)
        {
            ExistingItemNumbersPath = filePath;
            UserName = userName;
            EmailServer = new SmtpClient();
            EmailServer.Port = port;
            EmailServer.Host = host;
            EmailServer.EnableSsl = true;
            EmailServer.UseDefaultCredentials = false;
            EmailServer.Credentials = new NetworkCredential(userName, password);
            EmailServer.DeliveryMethod = SmtpDeliveryMethod.Network;
        }
        public INotificationData PrepareData(ListedItem listedItem)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod();
            var fullName = string.Format("{0}.{1}({2})", method.ReflectedType.FullName, method.Name, string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));


            INotificationData notificationData = new EmailNotificationData();
            notificationData.Subject = string.Format("NEW ITEM LISTED - {0}", listedItem.Name);
            // format email message body
            notificationData.Body = $@"{ listedItem.Link }

{ listedItem.Name }

{ listedItem.Condition }

${ listedItem.Price }";

            Tracer.PrintDetailedTrace(fullName, string.Format(notificationData.Body));

            return notificationData;
        }

        public void SendMessage(INotificationData message, string destinationEmail, ListedItem item)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(UserName);
                mail.To.Add(new MailAddress(destinationEmail));
                mail.Body = message.Body;
                mail.Subject = message.Subject;

                EmailServer.Send(mail);
                MarkItemAsSeen(item);
            }
            catch (Exception ex)
            {
                Tracer.PrintDetailedException(ex);
            }
        }

        public void MarkItemAsSeen(ListedItem listedItem)
        {
            using (StreamWriter sw = File.AppendText(ExistingItemNumbersPath))
            {
                sw.WriteLine(listedItem.ItemNumber);
            }
        }
    }
}
