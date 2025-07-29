using System.ComponentModel.DataAnnotations;

namespace GrandHotelPetrichMVC.ViewModels.Admin.Amenity
{
    public class AddAmenityViewModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
    }

}
