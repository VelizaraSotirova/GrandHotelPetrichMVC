using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Review;

namespace GrandHotelPetrichMVC.Data.Models
{
    public class Review
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(UserIdMaxLength)]
        public string UserId { get; set; } = null!;

        public Guid? RoomId { get; set; } // Optional - can be general hotel review

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(CommentMaxLength)]
        public string Comment { get; set; } = null!;

        public bool IsApproved { get; set; } = false;
        public bool IsFeatured { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(RoomId))]
        public virtual Room? Room { get; set; } // Nullable because it can be general hotel review
    }
}
