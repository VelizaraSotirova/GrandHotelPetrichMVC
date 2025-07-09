namespace GrandHotelPetrichMVC.ViewModels.Guests.Booking
{
    public class AvailableRoomViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal PricePerNight { get; set; }

        public string ImageUrl { get; set; } = null!;

        public List<string> Features { get; set; } = new();
    }
}
