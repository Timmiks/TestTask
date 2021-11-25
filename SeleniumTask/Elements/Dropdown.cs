using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace SeleniumTask.Elements
{
    public class Dropdown : BaseElement
    {
        public bool IsMultiselectable { get; }

        private BaseElement expandButton;
        public BaseElement ExpandButton
        {
            get
            {
                if (expandButton == null)
                {
                    expandButton = FindElement(By.CssSelector("button"));
                }
                return expandButton;
            }
        }

        public Dropdown(By by, bool isMultiselectable) : base(by)
        {
            this.IsMultiselectable = isMultiselectable;
        }

        private bool IsExpanded => GetAttribute("class").Contains("show");

        private string SelectedItem => Text;

        public void Expand()
        {
            if(!IsExpanded)
            {
                ExpandButton.Click();
            }
        }

        public void SelectItem(string item)
        {
            if (!IsMultiselectable && SelectedItem != item)
            {
                Expand();
                FindElements(By.CssSelector(".dropdown-item,.custom-checkbox")).First(i => i.Text == item).Click();
                new DefaultWait<Dropdown>(this) { Timeout = TimeSpan.FromSeconds(5), PollingInterval = TimeSpan.FromMilliseconds(200) }
                .Until(d => !d.IsExpanded);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public void SelectItems(params string[] items)
        {
            if (IsMultiselectable && SelectedItem.Split(new[] { ", " }, StringSplitOptions.None) != items)
            {
                Expand();
                foreach (var item in FindElements(By.CssSelector(".custom-checkbox")).Where(i => items.Contains(i.Text)))
                {
                    item.Click();
                }
                ExpandButton.Click();
                new DefaultWait<Dropdown>(this) { Timeout = TimeSpan.FromSeconds(5), PollingInterval = TimeSpan.FromMilliseconds(200) }
                .Until(d => !d.IsExpanded);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
