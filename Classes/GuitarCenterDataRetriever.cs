using GuitarCenterGearFinder.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Windows.Forms;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace GuitarCenterGearFinder.Classes
{
    public class GuitarCenterDataRetriever : IGuitarCenterDataRetriever, IDisposable
    {
        IWebDriver Driver;
        private bool Disposed;
        public string StartingUrl { get; set; }
        public string AlreadySentItemsFilePath { get; set; } 
        public HashSet<double> AlreadySentItems { get; set; } = new HashSet<double>();
        public int TimeoutSeconds { get; set; }

        public GuitarCenterDataRetriever(string url, string alreadySentItemsFilePath, int timeoutSeconds)
        {
            StartingUrl = url;
            Initialize(alreadySentItemsFilePath);
            TimeoutSeconds = timeoutSeconds;
        }

        public void Initialize(string alreadySentItemsFilePath)
        {
            try
            {
                new DriverManager().SetUpDriver(new ChromeConfig());
                ChromeOptions options = new ChromeOptions();
                options.AddArguments("headless", "log-level=3");
                Driver = new ChromeDriver(options);
            }
            catch (Exception ex)
            {
                Tracer.PrintDetailedException(ex);
            }

            Disposed = false;
            if (!File.Exists(alreadySentItemsFilePath))
            {
                File.CreateText(alreadySentItemsFilePath);
            }
            AlreadySentItemsFilePath = alreadySentItemsFilePath;
        }

        public IEnumerable<ListedItem> ListedItemsFound(string searchTerm)
        {
            IList<ListedItem> itemsFound = new List<ListedItem>();

            Driver.Navigate().GoToUrl(StartingUrl);

            IWebElement searchTextBox = Driver.FindElement(By.Name("advancedSearchTerms_used"));
            searchTextBox.Clear();
            searchTextBox.SendKeys(searchTerm);
            searchTextBox.SendKeys(OpenQA.Selenium.Keys.Enter);

            var elements = Driver.FindElements(By.ClassName("productTitle"));

            IList<string> gearUrls = new List<string>();

            // get all urls
            if (elements != null && elements.Any())
            {
                foreach (var element in elements)
                {
                    gearUrls.Add(element.FindElement(By.TagName("a")).GetAttribute("href"));
                }
            }

            // Get Item Information from each url
            foreach (string url in gearUrls)
            {
                try
                {
                    Driver.Navigate().GoToUrl(url);

                    double itemNumber = Convert.ToDouble(Driver.FindElement(By.XPath("//*[@id=\"PDPRightRailWrapper\"]/div[1]/div[3]/span[1]/span"), TimeoutSeconds).Text);
                    string itemName = Driver.FindElement(By.XPath("//*[@id=\"PDPRightRailWrapper\"]/div[1]/div[2]/h1"), TimeoutSeconds).Text;
                    string condition = Driver.FindElement(By.XPath("//*[@id=\"PDPRightRailWrapper\"]/div[3]/div[1]/span/span"), TimeoutSeconds).Text;


                    decimal price = decimal.Parse(Driver.FindElement(By.XPath("//*[@id=\"PDPRightRailWrapper\"]/div[3]/div[2]/div/span"), TimeoutSeconds).Text, System.Globalization.NumberStyles.Currency);


                    itemsFound.Add(new ListedItem(itemNumber, itemName, condition, price, url));
                }
                catch (Exception ex)
                {
                    Tracer.PrintDetailedException(ex);
                }
            }

            return itemsFound;
        }

        public IEnumerable<ListedItem> GetNewListedItems(string searchTerm)
        {
            GetSentItemNumbers();
            return ListedItemsFound(searchTerm).Where(i => !AlreadySentItems.Any(e => e == i.ItemNumber));
        }

        private void GetSentItemNumbers()
        {
            // Open the file to read from.
            using (StreamReader sr = File.OpenText(AlreadySentItemsFilePath))
            {
                string item = string.Empty;
                while ((item = sr.ReadLine()) != null)
                {
                    if (Double.TryParse(item, out double value) && !AlreadySentItems.Contains(value))
                    {
                        AlreadySentItems.Add(value);
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    if (Driver != null)
                    {
                        Driver.Dispose();
                    }
                }

                Disposed = true;
            }
        }
    }
}
