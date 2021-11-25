using System;
using System.Collections.Generic;
using System.Linq;

namespace SeleniumTask.PageObjects
{
    public static class PageCache
    {
        private static List<BasePage> Pages { get; } = new List<BasePage>();

        public static T Get<T>() where T : BasePage
        {
            var cachedInstanceCollection = Pages.OfType<T>();
            if (!cachedInstanceCollection.Any())
            {
                Pages.Add(Activator.CreateInstance<T>());
            }
            return cachedInstanceCollection.First();
        }
    }
}
