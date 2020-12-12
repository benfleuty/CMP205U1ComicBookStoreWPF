using DundeeComicBookStore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DundeeComicBookStore.Models
{
    public class BasketModel
    {
        /// <summary>
        /// ACCESS VIA PUBLIC METHOD
        /// </summary>
        private Dictionary<IProduct, int> _items;

        /// <summary>
        /// Method for getting and setting the basket's products and quantities
        /// </summary>
        public Dictionary<IProduct, int> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        /// <summary>
        /// Create a new instance and initialise Items
        /// </summary>
        public BasketModel()
        {
            Items = new Dictionary<IProduct, int>();
        }

        /// <summary>
        /// Create a new instance with the given dictionary of products
        /// </summary>
        /// <param name="products">Products stored in a dictionary of IProduct,int</param>
        public BasketModel(Dictionary<IProduct, int> products)
        {
            Items = products;
        }

        /// <summary>
        /// Create a new instance with product & quantity
        /// </summary>
        /// <param name="product">An IProduct object</param>
        /// <param name="quantity">The quantity of item to order as int</param>
        public BasketModel(IProduct product, int quantity)
        {
            Items = new Dictionary<IProduct, int>
            {
                { product, quantity }
            };
        }

        /// <summary>
        /// Add the given product and quantity to Items
        /// </summary>
        /// <param name="product">An IProduct object</param>
        /// <param name="quantity">The quantity of item to order as int</param>
        public void Add(IProduct product, int quantity)
        {
            if (Items.ContainsKey(product)) Items[product] += quantity;
            else Items.Add(product, quantity);
        }

        /// <summary>
        /// Add the given values in the dictionary to Items
        /// </summary>
        /// <param name="products">Products stored in a dictionary of IProduct,int</param>
        public void Add(Dictionary<IProduct, int> products)
        {
            foreach (var kvp in products)
            {
                if (Items.ContainsKey(kvp.Key)) Items[kvp.Key] += kvp.Value;
                else Items.Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Set Items to the given dictionary
        /// </summary>
        /// <param name="products">Products stored in a dictionary of IProduct,int</param>
        public void Set(Dictionary<IProduct, int> products)
        {
            Items = products;
        }

        /// <summary>
        /// Sets the quantity of the given product as the given amount
        /// </summary>
        /// <param name="target">The IProduct object to alter</param>
        /// <param name="value">The value to be set as the new quantity</param>
        public void SetQuantity(IProduct target, int value)
        {
            Items[target] = value;
        }

        /// <summary>
        /// Gets Items.Count
        /// </summary>
        /// <returns>The number of products in the basket</returns>
        public int Count(bool unique = false)
        {
            if (unique) return Items.Count;

            int count = 0;
            foreach (var kvp in Items)
                count += kvp.Value;
            return count;
        }
    }
}