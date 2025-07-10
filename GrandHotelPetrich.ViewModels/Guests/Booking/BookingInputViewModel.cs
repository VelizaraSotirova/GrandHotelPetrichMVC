using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GrandHotelPetrichMVC.ViewModels.Guests.Booking
{
    public class BookingInputViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Range(1, 6)]
        public int NumberOfGuests { get; set; }

        public Guid? SelectedRoomId { get; set; }

        public Guid? SelectedPaymentMethodId { get; set; }

        public string? SpecialRequests { get; set; }

        public List<SelectListItem> PaymentMethods { get; set; } = new();
        public List<AvailableRoomViewModel> AvailableRooms { get; set; } = new();
    }
}
