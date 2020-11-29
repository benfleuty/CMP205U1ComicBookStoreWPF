using DundeeComicBookStore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DundeeComicBookStore.Models
{
    internal class StaffModel : IUser
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public AddressModel Address { get; set; }
        public string ProfilePictureSource { get; set; }
        public uint RewardPoints { get; set; }

        [Flags]
        public enum StaffPermissions
        {
            None = 0,
            ReadCustomerData = 1,
            WriteCustomerData = 1 << 1,
            DeleteCustomerData = 1 << 2,

            ReadStockData = 1 << 3,
            WriteStockData = 1 << 4,
            DeleteStockData = 1 << 5,

            AccessEmployeeData = 1 << 6,

            CanReadAll = ReadCustomerData & ReadStockData,
            CanWriteAll = WriteCustomerData & WriteStockData,
            CanDeleteAll = DeleteCustomerData & DeleteStockData,

            AllOnCustomerData = ReadCustomerData & WriteCustomerData & DeleteCustomerData,

            AllOnStockData = ReadStockData & WriteStockData & DeleteStockData,

            All = AllOnCustomerData & AllOnStockData & AccessEmployeeData
        }

        public byte Permissions { get; set; }
    }
}