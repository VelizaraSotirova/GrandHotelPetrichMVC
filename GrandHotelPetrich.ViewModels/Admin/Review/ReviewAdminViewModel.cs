using GrandHotelPetrichMVC.GCommon.Enums;

namespace GrandHotelPetrichMVC.ViewModels.Admin.Review
{
    public class ReviewAdminViewModel
    {
        public Guid Id { get; set; }
        public string UserFullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? RoomName { get; set; } // Can be null if it's a general review
        public int Rating { get; set; }
        public string Title { get; set; } = null!;
        public string Comment { get; set; } = null!;
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
