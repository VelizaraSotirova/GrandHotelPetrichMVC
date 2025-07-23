using System.ComponentModel.DataAnnotations;

namespace GrandHotelPetrichMVC.ViewModels.Guests.Review
{
    public class ReviewDisplayViewModel
    {
        public string UserFullName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFeatured { get; set; }
    }
}
