namespace GrandHotelPetrichMVC.ViewModels.Guests.Booking
{
    public class MyBookingsViewModel
    {
        public string Filter { get; set; } = "All"; // "All", "Active", "Passed"
        public List<BookingDisplayViewModel> Bookings { get; set; } = new();

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
