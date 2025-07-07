using System.ComponentModel.DataAnnotations;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Gallery;

namespace GrandHotelPetrichMVC.Data.Models
{
    public class GalleryCategory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(CategoryMaxLength)]
        public string Name { get; set; } = null!;

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public ICollection<Gallery> Galleries { get; set; } = new List<Gallery>();
    }

}
