using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.ViewModels.Guests.Booking;
using System.ComponentModel.DataAnnotations;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Booking;

namespace GrandHotelPetrichMVC.ViewModels.Receptionists.Booking
{
    public class ReceptionistBookingSearchViewModel : IValidatableObject
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Range(MinNumberOfGuests, MaxNumberOfGuests)]
        public int NumberOfGuests { get; set; }

        public RoomType? RoomType { get; set; }

        public List<AvailableRoomViewModel> AvailableRooms { get; set; } = new();


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CheckInDate >= CheckOutDate)
            {
                yield return new ValidationResult(
                    "Check-in date must be before check-out date.",
                    new[] { nameof(CheckInDate), nameof(CheckOutDate) });
            }

            if (CheckInDate.Date < DateTime.Today)
            {
                yield return new ValidationResult(
                    "Check-in date cannot be in the past.",
                    new[] { nameof(CheckInDate) });
            }
        }
    }
}
