using GrandHotelPetrichMVC.GCommon.Enums;

namespace GrandHotelPetrichMVC.ViewModels.Receptionists.Room
{
    public class RoomStatusViewModel
    {
        public Guid RoomId { get; set; }
        public string RoomName { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public RoomStatus Status { get; set; }

        public DateTime? LastCheckOutDate { get; set; }
    }
}
