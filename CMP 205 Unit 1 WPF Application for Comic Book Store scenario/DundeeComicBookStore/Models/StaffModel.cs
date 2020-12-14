using DundeeComicBookStore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DundeeComicBookStore.Models
{
    public class StaffModel : IUser
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";

        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public string ProfilePictureSource { get; set; }
        public uint RewardPoints { get; set; }

        [Flags]
        public enum StaffPermissions
        {
            None = 0,                   // 0000 0000 // 0x00
            ReadCustomerData = 1,       // 0000 0001 // 0x01
            WriteCustomerData = 2,      // 0000 0010 // 0x02
            DeleteCustomerData = 4,     // 0000 0100 // 0x04

            ReadStockData = 8,          // 0000 1000 // 0x08
            WriteStockData = 16,        // 0001 0000 // 0x10
            DeleteStockData = 32,       // 0010 0000 // 0x20

            AccessEmployeeData = 64,    // 0100 0000 // 0x40

            CanReadAll = ReadCustomerData & ReadStockData,          // 0000 1001 // 0x09 // 9
            CanWriteAll = WriteCustomerData & WriteStockData,       // 0001 0010 // 0x12 // 18
            CanDeleteAll = DeleteCustomerData & DeleteStockData,    // 0010 0100 // 0x24 // 36

            // 0000 0111 // 0x07 // 7
            AllOnCustomerData = ReadCustomerData & WriteCustomerData & DeleteCustomerData,

            // 0011 1000 // 0x38 // 56
            AllOnStockData = ReadStockData & WriteStockData & DeleteStockData,

            // 0111 1111 // 0x7F // 127
            All = AllOnCustomerData & AllOnStockData & AccessEmployeeData
        }

        public byte Permissions { get; set; }

        public bool IsStaff => true;
    }
}