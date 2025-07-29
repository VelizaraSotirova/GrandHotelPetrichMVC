using System.ComponentModel.DataAnnotations;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Revenue;

namespace GrandHotelPetrichMVC.Data.Models
{
    public class RevenueSource
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(RevenueSourceNameMaxLength)]
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; } = 0;

        public virtual ICollection<Revenue> Revenues { get; set; } = new List<Revenue>();
    }

}
