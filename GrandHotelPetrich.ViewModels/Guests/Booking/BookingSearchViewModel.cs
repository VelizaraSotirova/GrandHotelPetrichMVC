using GrandHotelPetrichMVC.GCommon.Enums;
using System.ComponentModel.DataAnnotations;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Booking;

namespace GrandHotelPetrichMVC.ViewModels.Guests.Booking
{
    public class BookingSearchViewModel : IValidatableObject
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        [Range(MinNumberOfGuests, MaxNumberOfGuests)]
        public int NumberOfGuests { get; set; }

        public RoomType? RoomType { get; set; }

        public List<AvailableRoomViewModel> AvailableRooms { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CheckInDate < DateTime.Today)
            {
                yield return new ValidationResult("Check-in date must be today or later.", new[] { nameof(CheckInDate) });
            }

            if (CheckOutDate <= CheckInDate)
            {
                yield return new ValidationResult("Check-out date must be after check-in date.", new[] { nameof(CheckOutDate) });
            }
        }
    }

}
