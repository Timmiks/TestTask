using SeleniumTask.PageObjects;
using System;
using System.Linq;

namespace SeleniumTask
{
    class Program
    {
        private static VacanciesPage VacanciesPage => PageCache.Get<VacanciesPage>();

        static void Main(string[] args)
        {
            //в этом задании не делал полноценную верификацию параметров
            //"Подсчитать количество выданных вакансий и сравнить с ожидаемым результатом."
            //здесь решил не создавать юнит тесты, вывел примитивный резалт сравнения в консоль

            args = args.Select(arg => arg.Split('=').Last()).ToArray();
            VacanciesPage.Open();
            VacanciesPage.SelectDropdownValue(VacanciesPage.DepartmentDropdown, args[0]);
            VacanciesPage.SelectDropdownValues(VacanciesPage.LanguagesDropdown, args[1].Split(','));
            var count = VacanciesPage.Vacancies.Count.ToString();
            Drivers.Driver.CloseProcess();
            

            var message = $"Expected:{args[2]}.Actual:{count}.Equal:{args[2] == count}";
            Console.WriteLine(message);
            Console.ReadKey();
        }
    }
}
