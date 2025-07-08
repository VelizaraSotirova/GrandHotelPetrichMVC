namespace GrandHotelPetrichMVC.ViewModels.Rooms
{
    public class RoomViewModel
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string RoomType { get; set; } = null!;
        public int Size { get; set; } // in m²
        public int Capacity { get; set; } // number of guests
        public string BedType { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string Tag { get; set; } = null!; // e.g., Luxury, Family, Popular
    }
}
