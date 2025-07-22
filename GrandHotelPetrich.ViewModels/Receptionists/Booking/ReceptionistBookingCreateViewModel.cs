using GrandHotelPetrichMVC.ViewModels.Guests.Booking;
using System.ComponentModel.DataAnnotations;

namespace GrandHotelPetrichMVC.ViewModels.Receptionists.Booking
{
    public class ReceptionistBookingCreateViewModel 
    {
        // Guest details
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        public string PhoneNumber { get; set; } = null!;


        // Booking details
        [Required]
        public Guid RoomId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Range(1, 10)]
        public int NumberOfGuests { get; set; }

        [Required]
        public Guid PaymentMethodId { get; set; }

        public decimal TotalAmount { get; set; }
        public List<PaymentMethodViewModel> PaymentMethods { get; set; } = new();
    }
}
