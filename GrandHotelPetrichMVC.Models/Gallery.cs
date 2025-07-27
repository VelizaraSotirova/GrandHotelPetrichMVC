using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Gallery;

namespace GrandHotelPetrichMVC.Data.Models
{
    public class Gallery
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(ImageUrlMaxLength)]
        public string? ImageUrl { get; set; }

        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [MaxLength(DescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(CategoryId))]
        public GalleryCategory Category { get; set; } = null!;
    }
}
