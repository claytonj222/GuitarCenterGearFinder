using GuitarCenterGearFinder.Classes;
using GuitarCenterGearFinder.Interfaces;
using System.Configuration;
using System.Net;
using System.Security;

namespace GuitarCenterGearFinder
{
    public class Program
    {
        static void Main(string[] args)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod();
            var fullName = string.Format("{0}.{1}({2})", method.ReflectedType.FullName, method.Name, string.Join(",", method.GetParameters().Select(o => string.Format("{0} {1}", o.ParameterType, o.Name)).ToArray()));

            string filePath = Path.Combine(ConfigurationManager.AppSettings.Get("AlreadyProcessedItemsFilePath"), "ItemsSeen.txt");

            using (StreamWriter w = File.AppendText(filePath));

            int timeoutSeconds = 5;
            int.TryParse(ConfigurationManager.AppSettings.Get("WebsiteTimeoutSeconds"), out timeoutSeconds);
            int sleepTimeMinutes = 60;
            int.TryParse(ConfigurationManager.AppSettings.Get("ProgramSleepTimeMinutes"), out sleepTimeMinutes);

            Tracer.FilePath = "/Application.Log";

            if (bool.TryParse(ConfigurationManager.AppSettings.Get("DoLog"), out bool doLog))
            {
                Tracer.DoLog = doLog;
            }
            else
            {
                Tracer.PrintDetailedException(new InvalidCastException(string.Format("Failed to convert from {0} to bool from App.config file. Defaulting to only logging exceptions.", ConfigurationManager.AppSettings.Get("DoLog"))));
                Tracer.DoLog = false;

            }

            Tracer.PrintDetailedTrace(fullName, "Starting up");
            Console.WriteLine("Starting up");

            var searchTerms = new List<string>(ConfigurationManager.AppSettings["SearchTerms"].Split(new char[] { ';' }));

            SecureString password = new NetworkCredential("", ConfigurationManager.AppSettings["AlertEmailSenderPassword"]).SecurePassword;
            int port = 587;
            if (!int.TryParse(ConfigurationManager.AppSettings["AlertEmailSenderSmptPort"], out port))
            {
                Tracer.PrintDetailedException(new InvalidCastException(string.Format("Failed to convert from {0} to int from App.config file. Defaulting smtp port to 587.", ConfigurationManager.AppSettings.Get("AlertEmailSenderSmptPort"))));

            }

            INotificationHandler sender = new EmailNotificationHandler(filePath, ConfigurationManager.AppSettings.Get("AlertEmailSender"), password, port, ConfigurationManager.AppSettings["AlertEmailSenderSmptHost"]);
            password.Dispose();

            while (true)
            {
                GuitarCenterDataRetriever retriever = new GuitarCenterDataRetriever(ConfigurationManager.AppSettings.Get("WebsiteUrl"), filePath, timeoutSeconds);

                List<ListedItem> itemsFound = new List<ListedItem>();

                foreach (var searchTerm in searchTerms)
                {
                    itemsFound.AddRange(retriever.GetNewListedItems(searchTerm));
                }

                Tracer.PrintDetailedTrace(fullName, string.Format("{0} new items found to notify user", itemsFound.Count()));
                Console.WriteLine(string.Format("{0} new items found to notify user", itemsFound.Count()));


                foreach (var item in itemsFound)
                {
                    var result = sender.PrepareData(item);
                    sender.SendMessage(result, ConfigurationManager.AppSettings["AlertEmailDestination"], item);
                }

                if (itemsFound.Count > 0)
                {
                    Tracer.PrintDetailedTrace(fullName, string.Format("Notifications to user sent"));
                    Console.WriteLine("Notifications to user sent");
                }

                retriever.Dispose();

                Console.WriteLine(string.Format("Sleeping for {0} minute(s)", sleepTimeMinutes));
                Tracer.PrintDetailedTrace(fullName, string.Format("Sleeping for {0} minute(s)", sleepTimeMinutes));

                Thread.Sleep(new TimeSpan(0, sleepTimeMinutes, 0));
            }
        }
    }
}
