using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumTask.Elements;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SeleniumTask.PageObjects
{
    public class VacanciesPage : BasePage
    {
        public BaseElement VacanciesCounter = new BaseElement(By.CssSelector(".text-secondary.pl-2"));
        public Dropdown DepartmentDropdown = new Dropdown(By.XPath("//div[@class = 'dropdown' and ./button[text() = 'Все отделы']]"), false);
        public Dropdown LanguagesDropdown = new Dropdown(By.XPath("//div[@class = 'dropdown' and ./button[text() = 'Все языки']]"), true);
        public IList<BaseElement> Vacancies = new BaseElements(By.CssSelector("a.card"));
        public BaseElement VacanciesHolder = new BaseElement(By.CssSelector(".h-100.d-flex"));

        protected override string PageUrl { get; set; } = "vacancies";

        public override void WaitForPageLoading()
        {
            _ = new DefaultWait<BaseElement>(VacanciesCounter) { Timeout = TimeSpan.FromSeconds(5), PollingInterval = TimeSpan.FromMilliseconds(200) }
            .Until(vc => vc.Displayed && vc.Text != "0");
        }

        public void SelectDropdownValue(Dropdown dd, string value)
        {
            var vacanciesContentBefore = VacanciesHolder.GetAttribute("innerHTML");
            dd.SelectItem(value);
            WaitForVacanciesToUpdate(vacanciesContentBefore);
        }

        public void SelectDropdownValues(Dropdown dd, params string[] values)
        {
            var vacanciesContentBefore = VacanciesHolder.GetAttribute("innerHTML");
            dd.SelectItems(values);
            WaitForVacanciesToUpdate(vacanciesContentBefore);
        }

        private void WaitForVacanciesToUpdate(string vacanciesContentBefore)
        {
            Stopwatch timer = Stopwatch.StartNew();
            while (vacanciesContentBefore == VacanciesHolder.GetAttribute("innerHTML") && timer.Elapsed < TimeSpan.FromSeconds(1))
            {

            }
        }
    }
}
