namespace DundeeComicBookStore.Models
{
    public class AddressModel
    {
        private string _houseNameOrNumber;

        public string HouseNameOrNumber
        {
            get { return _houseNameOrNumber; }
            set { _houseNameOrNumber = value; }
        }

        private string _nameOfRoad;

        public string NameOfRoad
        {
            get { return _nameOfRoad; }
            set { _nameOfRoad = value; }
        }

        private string _postcode;

        public string Postcode
        {
            get { return _postcode; }
            set { _postcode = value; }
        }
    }
}