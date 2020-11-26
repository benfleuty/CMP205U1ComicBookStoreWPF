using System;
using System.Collections.Generic;
using System.Text;

namespace DundeeComicBookStore.Models
{
    public abstract class UserBase
    {
        private int _id;

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
    }
}