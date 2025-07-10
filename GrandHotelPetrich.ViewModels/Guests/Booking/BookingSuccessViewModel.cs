namespace GrandHotelPetrichMVC.ViewModels.Guests.Booking
{
    public class BookingSuccessViewModel
    {
        public Guid BookingId { get; set; }
        public string RoomName { get; set; } = null!;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? SpecialRequests { get; set; }
        public string BookingStatus { get; set; } = "Confirmed";
        public string PaymentStatus { get; set; } = "Paid";
    }
}
