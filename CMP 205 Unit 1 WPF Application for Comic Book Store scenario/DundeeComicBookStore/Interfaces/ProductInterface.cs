using System;
using System.Collections.Generic;
using System.Text;

namespace DundeeComicBookStore.Interfaces
{
    public interface IProduct
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public uint UnitPrice { get; set; }
        public uint UnitsInStock { get; set; }
        public uint UnitCost { get; set; }
    }
}