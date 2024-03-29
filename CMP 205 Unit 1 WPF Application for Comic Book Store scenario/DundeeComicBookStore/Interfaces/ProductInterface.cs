﻿namespace DundeeComicBookStore.Interfaces
{
    public interface IProduct
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public decimal UnitCost { get; set; }
    }
}