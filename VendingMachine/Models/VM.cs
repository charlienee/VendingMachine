namespace VendingMachine.Models
{
    public class VM
    {
        public int ID_VM { get; set; }
        public string Address { get; set; }
        public string PlaceDescription { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string Status { get; set; }

        public VM(int id, string address, string placeDescription, string model, string brand, string status)
        {
            ID_VM = id;
            Address = address;
            PlaceDescription = placeDescription;
            Model = model;
            Brand = brand;
            Status = status;
        }
    }
}