using DundeeComicBookStore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DundeeComicBookStore.Models
{
    public class OrderModel
    {
        public OrderModel()
        {
            Basket = new BasketModel();
        }

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

        private IUser _user;

        public IUser User
        {
            get { return _user; }
            set { _user = value; }
        }

        private bool _homeDelivery;

        public bool HomeDelivery
        {
            get { return _homeDelivery; }
            set { _homeDelivery = value; }
        }

        private PaymentType _paymentMethod;
        public PaymentType PaymentMethod { get => _paymentMethod; set => _paymentMethod = value; }

        public enum PaymentType
        {
            Card,
            Cash
        }

        public bool Discounted
        {
            get
            {
                return DBAccessHelper.DiscountOrder(User);
            }
        }

        public decimal Total
        {
            get
            {
                decimal total = Basket.Total;
                if (Discounted) total *= 0.75m;
                if (HomeDelivery) total += 4.99m;
                return total;
            }
        }
    }
}