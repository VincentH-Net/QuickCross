using SampleApp.Shared.Models;
using System.Collections.Generic;

namespace SampleApp.Shared.Services
{
    public class SampleItemService
    {
        private List<SampleItem> _items = new List<SampleItem>(new SampleItem[] { 
                new SampleItem { Id = 1, Title = "One", Description = "First item" },
                new SampleItem { Id = 2, Title = "Two", Description = "Second item" },
                new SampleItem { Id = 3, Title = "Three", Description = "Third item" }
        });

        private int _newId = 4;

        public List<SampleItem> GetItems()
        {
            return _items;
        }

        public SampleItem AddItem(string title, string description)
        {
            var item = new SampleItem();
            item.Id = _newId++;
            item.Title = title;
            item.Description = description;
            _items.Add(item);
            return item;
        }

        public SampleItem UpdateItem(SampleItem item)
        {
            SampleItem existingItem = _items.Find(i => { return i.Id == item.Id; });
            existingItem.Title = item.Title;
            existingItem.Description = item.Description;
            return existingItem;
        }

        public void RemoveItem(SampleItem item)
        {
            _items.Remove(item);
        }
    }
}
