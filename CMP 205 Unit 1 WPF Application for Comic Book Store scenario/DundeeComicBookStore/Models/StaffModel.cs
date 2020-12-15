using DundeeComicBookStore.Interfaces;
using System;
using System.Collections;

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

        public enum Permission
        {
            // array access is 0-7
            // perms are stored 7-0
            All,                // [0] --> 7

            AccessEmployeeData, // [1] --> 6
            DeleteStockData,    // [2] --> 5
            WriteStockData,     // [3] --> 4
            ReadStockData,      // [4] --> 3
            DeleteCustomerData, // [5] --> 2
            WriteCustomerData,  // [6] --> 1
            ReadCustomerData    // [7] --> 0
        }

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

            CanReadAll = ReadCustomerData + ReadStockData,          // 0000 1001 // 0x09 // 9
            CanWriteAll = WriteCustomerData + WriteStockData,       // 0001 0010 // 0x12 // 18
            CanDeleteAll = DeleteCustomerData + DeleteStockData,    // 0010 0100 // 0x24 // 36

            // 0000 0111 // 0x07 // 7
            AllOnCustomerData = ReadCustomerData + WriteCustomerData + DeleteCustomerData,

            // 0011 1000 // 0x38 // 56
            AllOnStockData = ReadStockData + WriteStockData + DeleteStockData,

            // 0111 1111 // 0x7F // 127
            All = AllOnCustomerData + AllOnStockData + AccessEmployeeData
        }

        private BitArray _permissions;

        public BitArray Permissions
        {
            get => _permissions;
            set => _permissions = value;
        }

        public bool Can(Permission perm)
        {
            byte permToCheck = (byte)perm;
            if (Permissions[permToCheck])
                return true;
            else return false;
        }

        public bool Can(StaffPermissions perm)
        {
            bool result;

            switch ((StaffPermissions)Convert.ToByte(perm))
            {
                case StaffPermissions.CanReadAll:
                    result = Can(Permission.ReadCustomerData)
                        && Can(Permission.ReadStockData);
                    break;

                case StaffPermissions.CanWriteAll:
                    result = Can(Permission.WriteCustomerData)
                        && Can(Permission.WriteStockData);
                    break;

                case StaffPermissions.CanDeleteAll:
                    result = Can(Permission.DeleteCustomerData)
                        && Can(Permission.DeleteStockData);
                    break;

                case StaffPermissions.AllOnCustomerData:
                    result = Can(Permission.ReadCustomerData)
                        && Can(Permission.WriteCustomerData)
                        && Can(Permission.DeleteCustomerData);
                    break;

                case StaffPermissions.AllOnStockData:
                    result = Can(Permission.ReadStockData)
                        && Can(Permission.WriteStockData)
                        && Can(Permission.DeleteStockData);
                    break;

                case StaffPermissions.All:
                    result = Can(StaffPermissions.AllOnCustomerData)
                        && Can(StaffPermissions.AllOnStockData)
                        && Can(Permission.AccessEmployeeData);
                    break;

                default:
                    result = Can((Permission)perm);
                    break;
            }

            return result;

            //byte permToCheck = (byte)perm;
            //if (Permissions[permToCheck])
            //    return true;
            //else return false;
        }

        public bool IsStaff => true;
    }
}