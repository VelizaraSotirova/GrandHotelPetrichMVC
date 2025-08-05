using GrandHotelPetrichMVC.Services.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Controllers
{
    public class GalleryController : Controller
    {
        private readonly IGalleryService _galleryService;

        public GalleryController(IGalleryService galleryService)
        {
            _galleryService = galleryService;
        }

        public async Task<IActionResult> Index()
        {
            var images = await _galleryService.GetAllImagesAsync();
            return View(images);
        }
    }
}
