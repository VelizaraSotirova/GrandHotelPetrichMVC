using GrandHotelPetrichMVC.GCommon.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Room;

namespace GrandHotelPetrichMVC.ViewModels.Admin.Room
{
    public class RoomCreateViewModel
    {
        public Guid? Id { get; set; }

        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; } = null!;

        public RoomType RoomType { get; set; }

        [Range(PricePerNightMin, PricePerNightMax)]
        public decimal PricePerNight { get; set; }

        [Range(MinCapacity, MaxConstCapacity)]
        public int MaxCapacity { get; set; }

        public BedConfiguration BedConfiguration { get; set; }

        [Required]
        [MaxLength(MaxDescriptionLength)]
        public string Description { get; set; } = null!;

        [Required]
        [MaxLength(ImageUrlMaxLength)]
        public string ImageUrl { get; set; } = null!;

        public RoomBadge Badge { get; set; }

        public List<Guid> SelectedAmenityIds { get; set; } = new();
        public List<SelectListItem> AvailableAmenities { get; set; } = new();
    }
}
