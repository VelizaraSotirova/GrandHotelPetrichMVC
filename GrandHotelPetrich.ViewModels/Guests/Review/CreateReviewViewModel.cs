using System.ComponentModel.DataAnnotations;
using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Review;

namespace GrandHotelPetrichMVC.ViewModels.Guests.Review
{
    public class CreateReviewViewModel
    {
        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(CommentMaxLength)]
        public string Comment { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; }

        public Guid? RoomId { get; set; } // Optional
    }

}
