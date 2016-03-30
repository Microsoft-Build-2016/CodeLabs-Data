using System.Collections.Generic;

namespace MSCorp.AdventureWorks.Web.Models
{
    public class ItemSet
    {
        private readonly List<ItemModel> _items;

        public ItemSet()
        {
            _items = new List<ItemModel>();
        }

        public IEnumerable<ItemModel> Items { get {return _items;} }

        public void Add(ItemModel itemModel)
        {
            _items.Add(itemModel);
        }
    }
}