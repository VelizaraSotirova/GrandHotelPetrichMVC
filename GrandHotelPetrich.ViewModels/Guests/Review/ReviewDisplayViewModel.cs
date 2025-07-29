using System.ComponentModel.DataAnnotations;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Review;

namespace GrandHotelPetrichMVC.ViewModels.Guests.Review
{
    public class ReviewDisplayViewModel
    {
        public string UserFullName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Comment { get; set; } = null!;

        [Range(MinRating, MaxRating)]
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFeatured { get; set; }
    }
}
