using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Admin.Gallery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin")]
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

        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            ViewBag.Categories = await _galleryService.GetCategoriesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(GalleryUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _galleryService.GetCategoriesAsync();
                return View(model);
            }

            var success = await _galleryService.UploadImageAsync(model, "images");
            if (success)
            {
                TempData["Success"] = "Image uploaded successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Image upload failed.");
            ViewBag.Categories = await _galleryService.GetCategoriesAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _galleryService.DeleteImageAsync(id);
            if (success)
                TempData["Success"] = "Image deleted.";
            else
                TempData["Error"] = "Failed to delete image.";

            return RedirectToAction(nameof(Index));
        }
    }
}
