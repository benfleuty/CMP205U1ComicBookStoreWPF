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
        private IUser _user;

        /// <summary>
        /// Method for getting and setting the basket's user
        /// </summary>
        public IUser User
        {
            get { return _user; }
            set { _user = value; }
        }

        /// <summary>
        /// ACCESS VIA PUBLIC METHOD
        /// </summary>
        private Dictionary<IProduct, uint> _items;

        /// <summary>
        /// Method for getting and setting the basket's products and quantities
        /// </summary>
        public Dictionary<IProduct, uint> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        /// <summary>
        /// Create a new instance with the owning user defined
        /// </summary>
        /// <param name="user">An IUser object</param>
        public BasketModel(IUser user)
        {
            User = user;
        }

        /// <summary>
        /// Create a new instance with the given dictionary of products
        /// </summary>
        /// <param name="products">Products stored in a dictionary of IProduct,uint</param>
        public BasketModel(Dictionary<IProduct, uint> products)
        {
            Items = products;
        }

        /// <summary>
        /// Create a new instance with the given user, and product & quantity
        /// </summary>
        /// <param name="user">An IUser object</param>
        /// <param name="product">An IProduct object</param>
        /// <param name="quantity">The quantity of item to order as uint</param>
        public BasketModel(IUser user, IProduct product, uint quantity)
        {
            User = user;
            Items.Add(product, quantity);
        }

        /// <summary>
        /// Create a new instance with the given user, and dictionary of products
        /// </summary>
        /// <param name="user">An IUser object</param>
        /// <param name="products">Products stored in a dictionary of IProduct,uint</param>
        public BasketModel(IUser user, Dictionary<IProduct, uint> products)
        {
            User = user;
            Items = products;
        }

        /// <summary>
        /// Add the given product and quantity to Items
        /// </summary>
        /// <param name="product">An IProduct object</param>
        /// <param name="quantity">The quantity of item to order as uint</param>
        public void Add(IProduct product, uint quantity)
        {
            Items.Add(product, quantity);
        }

        /// <summary>
        /// Add the given values in the dictionary to Items
        /// </summary>
        /// <param name="products">Products stored in a dictionary of IProduct,uint</param>
        public void Add(Dictionary<IProduct, uint> products)
        {
            foreach (var kvp in products)
                Items.Add(kvp.Key, kvp.Value);
        }

        /// <summary>
        /// Set Items to the given dictionary
        /// </summary>
        /// <param name="products">Products stored in a dictionary of IProduct,uint</param>
        public void Set(Dictionary<IProduct, uint> products)
        {
            Items = products;
        }

        /// <summary>
        /// Sets the quantity of the given product as the given amount
        /// </summary>
        /// <param name="target">The IProduct object to alter</param>
        /// <param name="value">The value to be set as the new quantity</param>
        public void SetQuantity(IProduct target, uint value)
        {
            Items[target] = value;
        }

        /// <summary>
        /// Gets Items.Count
        /// </summary>
        /// <returns>The number of products in the basket</returns>
        public int Count()
        {
            return Items.Count;
        }
    }
}