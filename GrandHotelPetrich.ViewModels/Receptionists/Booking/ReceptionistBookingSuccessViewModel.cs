using System.ComponentModel.DataAnnotations;

namespace GrandHotelPetrichMVC.ViewModels.Receptionists.Booking
{
    public class ReceptionistBookingSuccessViewModel
    {
        [Required]
        public string GuestFullName { get; set; } = null!;

        [Required]
        public string GuestEmail { get; set; } = null!;

        [Required]
        public string RoomName { get; set; } = null!;

        [Required]
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = null!;
    }
}
