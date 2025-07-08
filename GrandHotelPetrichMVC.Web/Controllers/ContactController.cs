using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ContactMessageFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var message = new ContactMessage
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.Phone,
                Subject = model.Subject,
                Message = model.Message,
            };

            _context.ContactMessages.Add(message);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your message has been sent successfully!";
            return RedirectToAction("Index");
        }
    }
}
