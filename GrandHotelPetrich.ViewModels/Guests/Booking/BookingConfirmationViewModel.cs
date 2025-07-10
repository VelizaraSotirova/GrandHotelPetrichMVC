using GrandHotelPetrichMVC.GCommon.Enums;
using System.ComponentModel.DataAnnotations;

namespace GrandHotelPetrichMVC.ViewModels.Guests.Booking
{
    public class BookingConfirmationViewModel
    {
        public Guid RoomId { get; set; }
        public RoomType RoomType { get; set; }
        public string Description { get; set; } = null!;
        public string? RoomImageUrl { get; set; }
        public decimal PricePerNight { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        public int NumberOfGuests { get; set; }
        public decimal TotalAmount { get; set; }

        [MaxLength(300)]
        public string? SpecialRequests { get; set; }

        [Required]
        public Guid PaymentMethodId { get; set; }

        public List<PaymentMethodViewModel> PaymentMethods { get; set; } = new();
    }

}
