using GrandHotelPetrichMVC.GCommon.Enums;

namespace GrandHotelPetrichMVC.ViewModels.Guests.Booking
{
    public class RoomDetailsViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal PricePerNight { get; set; }

        public int Capacity { get; set; }

        public RoomType RoomTypeName { get; set; }

        public string? ImageUrl { get; set; }
    }
}
