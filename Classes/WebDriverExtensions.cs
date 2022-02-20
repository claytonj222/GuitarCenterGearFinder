using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace GuitarCenterGearFinder.Classes
{
    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds = 1)
        {
            IWebElement elementFound = null;

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));

            try
            {
                wait.Until<IWebElement>((d) =>
                {
                    try
                    {
                        return d.FindElement(by);
                    }
                    catch (NoSuchElementException e)
                    {
                        return null;
                    }
                });
            }
            catch (WebDriverTimeoutException e)
            {
                return null;
            }
            catch (NullReferenceException e)
            {
                return null;
            }

            elementFound = wait.Until<IWebElement>(d => driver.FindElement(by));

            return elementFound;
        }

        public static bool DoesElementExist(this IWebDriver driver, By by, int timeoutInSeconds = 0)
        {
            //return driver.FindElement(value, timeoutInSeconds) != null ? true : false;
            try
            {
                driver.FindElement(by);
            }
            catch (NoSuchElementException e)
            {
                return false;
            }
            return true;
        }
    }
}
