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
        public string Address { get; set; }
        public string ProfilePictureSource { get; set; }
        public uint RewardPoints { get; set; }

        [Flags]
        public enum StaffPermissions
        {
            None = 0,                       // 0000 0000
            ReadCustomerData = 1,           // 0000 0001
            WriteCustomerData = 1 << 1,     // 0000 0010
            DeleteCustomerData = 1 << 2,    // 0000 0100

            ReadStockData = 1 << 3,         // 0000 1000
            WriteStockData = 1 << 4,        // 0001 0000
            DeleteStockData = 1 << 5,       // 0010 0000

            AccessEmployeeData = 1 << 6,    // 0100 0000

            CanReadAll = ReadCustomerData & ReadStockData,          // 0000 1001
            CanWriteAll = WriteCustomerData & WriteStockData,       // 0001 0010
            CanDeleteAll = DeleteCustomerData & DeleteStockData,    // 0010 0100

            AllOnCustomerData = ReadCustomerData & WriteCustomerData & DeleteCustomerData, // 0000 0111

            AllOnStockData = ReadStockData & WriteStockData & DeleteStockData,             // 0011 1000

            All = AllOnCustomerData & AllOnStockData & AccessEmployeeData                  // 0111 1111
        }

        public byte Permissions { get; set; }

        public bool IsStaff => true;
    }
}