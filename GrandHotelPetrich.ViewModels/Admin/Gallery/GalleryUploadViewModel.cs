using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GrandHotelPetrichMVC.ViewModels.Admin.Gallery
{
    public class GalleryUploadViewModel
    {
        [Required]
        public IFormFile ImageFile { get; set; } = null!;

        public Guid CategoryId { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }

        public string? Description { get; set; }
    }
}
