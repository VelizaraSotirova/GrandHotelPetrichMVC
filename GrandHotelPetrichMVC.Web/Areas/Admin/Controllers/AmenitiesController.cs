using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Admin.Amenity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin")]
    public class AmenitiesController : Controller
    {
        private readonly IAmenityService _amenityService;

        public AmenitiesController(IAmenityService amenityService)
        {
            _amenityService = amenityService;
        }

        public async Task<IActionResult> Index()
        {
            var amenities = await _amenityService.GetAllAsync();
            return View(amenities);
        }

        [HttpPost]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await _amenityService.DeactivateAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Activate(Guid id)
        {
            await _amenityService.ActivateAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddAmenityViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddAmenityViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _amenityService.AddAsync(model);
            if (!success)
            {
                ModelState.AddModelError(nameof(model.Name), "Amenity already exists.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }

}
