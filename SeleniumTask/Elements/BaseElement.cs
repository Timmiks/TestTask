using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumTask.Drivers;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace SeleniumTask.Elements
{
    //т.к. в селениуме для c# депрекейтнули PageFactory и ExpectedConditions то далее:
    // 1. Писать собственный PageFactory не решил из-за размеров задачи
    // 2. Создан врап класс для IWebelement, с отложенным получением элемента,
    // вся внутренняя работа идет через проперти, где вшит Staleness чек в чистом виде, соответствуя коду из сорсов селениума
    // 3. Все вейты используются в чистом виде, ImplicitTimeout решил не использовать из-за размеров задачи
    public class BaseElement
    {
        private IWebElement instance = null;
        private BaseElement searchContext = null;
        private By by;
        
        public BaseElement(By by)
        {
            this.by = by;
        }

        private IWebElement Instance 
        { 
            get
            {
                if (instance == null)
                {
                    return instance = new WebDriverWait(Driver.Instance, TimeSpan.FromSeconds(10)) { PollingInterval = TimeSpan.FromMilliseconds(200) }
                    .Until(d => searchContext?.Instance.FindElement(by) ?? d.FindElement(by));
                }
                else
                {

                    try
                    {
                        _ = instance.Enabled;
                        return instance;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return instance = new WebDriverWait(Driver.Instance, TimeSpan.FromSeconds(10)) { PollingInterval = TimeSpan.FromMilliseconds(200) }
                        .Until(d => searchContext?.Instance.FindElement(by) ?? d.FindElement(by));
                    }
                }
            }
        }

        public string TagName => Instance.TagName;

        public string Text => Instance.Text;

        public bool Enabled => Instance.Enabled;

        public bool Selected => Instance.Selected;

        public Point Location => Instance.Location;

        public Size Size => Instance.Size;

        public bool Displayed => Instance.Displayed;

        public void Clear()
        {
            Instance.Clear();
        }

        public void Click()
        {
            Instance.Click();
        }

        public BaseElement FindElement()
        {
            var el = new BaseElement(By.XPath(""));
            return el;
        }

        public BaseElement FindElement(By by)
        {
            var el = new BaseElement(by);
            el.searchContext = this;
            return el;
        }

        public ReadOnlyCollection<BaseElement> FindElements(By by)
        {
            return new ReadOnlyCollection<BaseElement>(Instance.FindElements(by).Select(e => new BaseElement(by) { instance = e, searchContext = this}).ToList());
        }

        public string GetAttribute(string attributeName)
        {
            return Instance.GetAttribute(attributeName);
        }

        public string GetCssValue(string propertyName)
        {
            return Instance.GetCssValue(propertyName);
        }

        public string GetDomAttribute(string attributeName)
        {
            return Instance.GetDomAttribute(attributeName);
        }

        public string GetDomProperty(string propertyName)
        {
            return Instance.GetDomProperty(propertyName);
        }

        [Obsolete]
        public string GetProperty(string propertyName)
        {
            return Instance.GetProperty(propertyName);
        }

        public ISearchContext GetShadowRoot()
        {
            return Instance.GetShadowRoot();
        }

        public void SendKeys(string text)
        {
            Instance.SendKeys(text);
        }

        public void Submit()
        {
            Instance.Submit();
        }
    }
}
