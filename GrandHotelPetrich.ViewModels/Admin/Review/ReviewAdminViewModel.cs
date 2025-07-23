using GrandHotelPetrichMVC.GCommon.Enums;

namespace GrandHotelPetrichMVC.ViewModels.Admin.Review
{
    public class ReviewAdminViewModel
    {
        public Guid Id { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? RoomName { get; set; } // Can be null if it's a general review
        public int Rating { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
