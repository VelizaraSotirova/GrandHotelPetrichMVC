using System.ComponentModel.DataAnnotations;
using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Amenity;

namespace GrandHotelPetrichMVC.ViewModels.Admin.Amenity
{
    public class AddAmenityViewModel
    {
        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;
    }
}
