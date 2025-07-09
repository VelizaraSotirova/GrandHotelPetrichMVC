using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GrandHotelPetrichMVC.ViewModels.Guests.Booking
{
    public class BookingInputModel
    {
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Range(1, 6, ErrorMessage = "Guests must be between 1 and 6.")]
        [Display(Name = "Guests")]
        public int Guests { get; set; }

        [Display(Name = "Room Type")]
        public int? RoomTypeId { get; set; }

        public List<SelectListItem> RoomTypes { get; set; } = new List<SelectListItem>();

        [Display(Name = "Special Requests")]
        [MaxLength(500)]
        public string? SpecialRequests { get; set; }

        // This will be populated with results from service after form submission
        public List<AvailableRoomViewModel> AvailableRooms { get; set; } = new List<AvailableRoomViewModel>();
    }
}
