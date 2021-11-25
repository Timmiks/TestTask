using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SeleniumTask.Elements
{
    //аналогично, решая проблемы отсутствия PageFactory и желания инициализировать через поля:
    // 1. Создан класс, врапающий коллекцию на основе BaseElement
    // 2. Интерфейсы скопированы исходя из информации о ReadOnlyCollection, которую возвращает дефолтный FindElements
    // 3. Часть пропертей и методов идут без public и через Explicit, таким образом скрывая их от вызывающих классов, по аналогии с ReadOnlyCollection
    public class BaseElements : IList<BaseElement>, ICollection<BaseElement>, IEnumerable<BaseElement>, IReadOnlyList<BaseElement>, IReadOnlyCollection<BaseElement>
    {
        private ReadOnlyCollection<BaseElement> collection = null;
        private By by;
        private readonly bool autoResetCache;

        public BaseElements(By by, bool autoResetCache)
        {
            this.by = by;
            this.autoResetCache = autoResetCache;
        }

        public BaseElements(By by) : this(by, false)
        {
            this.by = by;
        }

        BaseElement IList<BaseElement>.this[int index] { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public BaseElement this[int index] { get => Collection[index]; set => ((IList<BaseElement>)Collection)[index] = value; }

        public int Count => Collection.Count;

        bool ICollection<BaseElement>.IsReadOnly => throw new NotSupportedException();

        private ReadOnlyCollection<BaseElement> Collection
        {
            get
            {
                if (autoResetCache)
                {
                    ResetCache();
                }
                if (collection == null)
                {
                    var html = new BaseElement(By.CssSelector("html"));
                    collection = new DefaultWait<BaseElement>(html){ Timeout = TimeSpan.FromSeconds(10), PollingInterval = TimeSpan.FromMilliseconds(200) }
                    .Until(h => h.FindElements(by));
                }
                return collection;
            }
        }

        public void ResetCache()
        {
            collection = null;
        }

        void ICollection<BaseElement>.Add(BaseElement item)
        {
            throw new NotSupportedException();
        }

        void ICollection<BaseElement>.Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(BaseElement item)
        {
            return Collection.Contains(item);
        }

        public void CopyTo(BaseElement[] array, int arrayIndex)
        {
            Collection.CopyTo(array, arrayIndex);
        }

        public IEnumerator<BaseElement> GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        public int IndexOf(BaseElement item)
        {
            return Collection.IndexOf(item);
        }

        void IList<BaseElement>.Insert(int index, BaseElement item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<BaseElement>.Remove(BaseElement item)
        {
            throw new NotSupportedException();
        }

        void IList<BaseElement>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Collection.GetEnumerator();
        }
    }
}
