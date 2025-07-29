using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactService _publicService;

        public ContactController(IContactService publicService)
        {
            _publicService = publicService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var email = User.Identity?.IsAuthenticated == true ? User.Identity.Name : null;
            var model = await _publicService.GetPrefilledContactFormAsync(email);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ContactMessageFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            await _publicService.SubmitContactFormAsync(model);

            TempData["Success"] = "Your message has been sent successfully!";
            return RedirectToAction("Index");
        }
    }
}
