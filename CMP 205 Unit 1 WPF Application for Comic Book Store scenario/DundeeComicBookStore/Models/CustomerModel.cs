using DundeeComicBookStore.Interfaces;

namespace DundeeComicBookStore.Models
{
    public class CustomerModel : IUser
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
        public byte Permissions { get; set; }
        public bool IsStaff => false;
    }
}