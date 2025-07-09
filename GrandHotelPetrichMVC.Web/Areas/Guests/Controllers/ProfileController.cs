using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Guests.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Areas.Guests.Controllers
{
    [Area("Guests")]
    [Authorize(Roles = "Customer")]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly UserManager<User> _userManager;

        public ProfileController(IProfileService profileService, UserManager<User> userManager)
        {
            _profileService = profileService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var profile = await _profileService.GetProfileAsync(user.Id);
            if (profile == null) return NotFound();

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var success = await _profileService.UpdateProfileAsync(user.Id, model);

            if (!success)
            {
                ModelState.AddModelError("", "Failed to update profile.");
                return View(model);
            }

            TempData["Success"] = "Your profile was updated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
