﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DundeeComicBookStore.Models
{
    public class OrderModel
    {
        private int _id;

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private BasketModel _basket;

        public BasketModel Basket
        {
            get { return _basket; }
            set { _basket = value; }
        }

        private DateTime _placedAt;

        public DateTime PlacedAt
        {
            get { return _placedAt; }
            set { _placedAt = value; }
        }

        private bool _complete;

        public bool Complete
        {
            get { return _complete; }
            set { _complete = value; }
        }

        private string _address;

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        private bool _beingEditied;

        public bool BeingEdited
        {
            get { return _beingEditied; }
            set { _beingEditied = value; }
        }
    }
}