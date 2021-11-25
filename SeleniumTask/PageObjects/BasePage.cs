using SeleniumTask.Drivers;

namespace SeleniumTask.PageObjects
{
    public abstract class BasePage
    {
        public BasePage()
        {
        }

        protected abstract string PageUrl { get; set; }

        public virtual void Open()
        {
            Driver.Instance.Navigate().GoToUrl(string.Format("https://careers.veeam.ru/{0}", PageUrl));
            WaitForPageLoading();
        }

        public abstract void WaitForPageLoading();
    }
}
