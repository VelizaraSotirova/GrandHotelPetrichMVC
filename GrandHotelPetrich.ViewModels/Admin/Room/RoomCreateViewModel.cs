using GrandHotelPetrichMVC.GCommon.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GrandHotelPetrichMVC.ViewModels.Admin.Room
{
    public class RoomCreateViewModel
    {
        public Guid? Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public RoomType RoomType { get; set; }

        [Range(1, 10000)]
        public decimal PricePerNight { get; set; }

        [Range(1, 20)]
        public int MaxCapacity { get; set; }

        public BedConfiguration BedConfiguration { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string ImageUrl { get; set; } = null!;

        public RoomBadge Badge { get; set; }

        public List<Guid> SelectedAmenityIds { get; set; } = new();
        public List<SelectListItem> AvailableAmenities { get; set; } = new();
    }
}
