using GrandHotelPetrichMVC.GCommon.Enums;
using System.ComponentModel.DataAnnotations;

namespace GrandHotelPetrichMVC.ViewModels.Guests.Booking
{
    public class BookingSearchViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        [Range(1, 6)]
        public int NumberOfGuests { get; set; }

        public RoomType? RoomType { get; set; }

        public List<AvailableRoomViewModel> AvailableRooms { get; set; } = new();
    }

}
