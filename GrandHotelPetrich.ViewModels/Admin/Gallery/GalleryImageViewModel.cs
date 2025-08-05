using GrandHotelPetrichMVC.Data.Models;

namespace GrandHotelPetrichMVC.ViewModels.Admin.Gallery
{
    public class GalleryImageViewModel
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = null!;

        public string? Description { get; set; }

        public string Title { get; set; } = null!;  
        public string CategoryName { get; set; } = null!;
    }
}
