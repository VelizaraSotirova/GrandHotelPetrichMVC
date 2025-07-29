using System.ComponentModel.DataAnnotations;
using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Review;

namespace GrandHotelPetrichMVC.ViewModels.Guests.Review
{
    public class CreateReviewViewModel
    {
        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(CommentMaxLength)]
        public string Comment { get; set; } = null!;

        [Range(MinRating, MaxRating)]
        public int Rating { get; set; }

        public Guid? RoomId { get; set; } // Optional
    }

}
