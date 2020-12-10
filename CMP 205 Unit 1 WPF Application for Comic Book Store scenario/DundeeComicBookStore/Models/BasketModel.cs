﻿using DundeeComicBookStore.Interfaces;
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
        /// Create a new instance with the owning user defined
        /// </summary>
        /// <param name="user">An IUser object</param>

        public BasketModel()
        {
            Items = new Dictionary<IProduct, int>();
        }

        public BasketModel(IUser user)
        {
            User = user;
            if (Items == null) Items = new Dictionary<IProduct, int>();
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
        /// Create a new instance with the given user, and product & quantity
        /// </summary>
        /// <param name="user">An IUser object</param>
        /// <param name="product">An IProduct object</param>
        /// <param name="quantity">The quantity of item to order as int</param>
        public BasketModel(IUser user, IProduct product, int quantity)
        {
            User = user;
            Items = new Dictionary<IProduct, int>
            {
                { product, quantity }
            };
        }

        /// <summary>
        /// Create a new instance with the given user, and dictionary of products
        /// </summary>
        /// <param name="user">An IUser object</param>
        /// <param name="products">Products stored in a dictionary of IProduct,int</param>
        public BasketModel(IUser user, Dictionary<IProduct, int> products)
        {
            User = user;
            Items = products;
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