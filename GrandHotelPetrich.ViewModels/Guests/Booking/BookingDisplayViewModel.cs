using GrandHotelPetrichMVC.GCommon.Enums;

namespace GrandHotelPetrichMVC.ViewModels.Guests.Booking
{
    public class BookingDisplayViewModel
    {
        public Guid Id { get; set; }
        public string RoomName { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }
        public BookingStatus BookingStatus { get; set; }
    }
}
