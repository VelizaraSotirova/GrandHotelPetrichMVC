using GrandHotelPetrichMVC.GCommon.Enums;

namespace GrandHotelPetrichMVC.ViewModels.Admin.Room
{
    public class RoomListViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public RoomType RoomType { get; set; }
        public decimal PricePerNight { get; set; }
        public int MaxCapacity { get; set; }
        public RoomBadge Badge { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsActive { get; set; }
        public RoomStatus Status { get; set; }
    }
}
