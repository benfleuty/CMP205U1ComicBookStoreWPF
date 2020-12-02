using DundeeComicBookStore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DundeeComicBookStore.Interfaces
{
    internal interface IUser
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public AddressModel Address { get; set; }
        public string ProfilePictureSource { get; set; }
        public uint RewardPoints { get; set; }
    }
}