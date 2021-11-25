using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using System.Linq;

namespace SeleniumTask.Drivers
{
    public static class Driver
    {
        private static IWebDriver instance = null;
        public static IWebDriver Instance 
        { 
            get
            {
                if (instance == null)
                {
                    var chromeDriverService = ChromeDriverService.CreateDefaultService();
                    chromeDriverService.HideCommandPromptWindow = true;
                    instance = new ChromeDriver(chromeDriverService);
                    instance.Manage().Window.Maximize();
                }
                return Instance = instance;

            }
            private set { }
       }

        internal static void CloseProcess()
        {
            using (var process = Process.GetProcessesByName("chromedriver").Single())
            {
                process.Kill();
                process.WaitForExit();
            }
        }
    }
}
