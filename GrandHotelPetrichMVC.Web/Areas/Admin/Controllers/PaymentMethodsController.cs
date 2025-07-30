using GrandHotelPetrichMVC.Services.Core;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Admin.Payment;
using GrandHotelPetrichMVC.ViewModels.Guests.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin")]
    public class PaymentMethodsController : Controller
    {
        private readonly IPaymentMethodService _service;

        public PaymentMethodsController(IPaymentMethodService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _service.GetAllMethodsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
                await _service.AddMethodAsync(name);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable(Guid id)
        {
            await _service.DisableMethodAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(Guid id)
        {
            await _service.ReactivateMethodAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
