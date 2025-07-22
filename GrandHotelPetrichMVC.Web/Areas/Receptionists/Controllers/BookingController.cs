using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Receptionists.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GrandHotelPetrichMVC.Web.Areas.Receptionists.Controllers
{
    [Area(nameof(Receptionists))]
    [Authorize(Roles = "Receptionist")]
    public class BookingController : Controller
    {
        private readonly IReceptionistService _receptionistService;

        public BookingController(IReceptionistService receptionistService)
        {
            _receptionistService = receptionistService;
        }

        [HttpGet]
        public IActionResult Search()
        {
            var model = new ReceptionistBookingSearchViewModel
            {
                CheckInDate = DateTime.UtcNow.Date,
                CheckOutDate = DateTime.UtcNow.AddDays(1).Date
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Search(ReceptionistBookingSearchViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            model.AvailableRooms = await _receptionistService.GetAvailableRoomsAsync(
                model.CheckInDate, model.CheckOutDate, model.NumberOfGuests, model.RoomType);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Book(Guid roomId, DateTime checkIn, DateTime checkOut, int guests)
        {
            var nights = (checkOut - checkIn).Days;
            var room = await _receptionistService.GetAvailableRoomsAsync(checkIn, checkOut, guests, null);
            var roomToBook = room.FirstOrDefault(r => r.Id == roomId);

            if (roomToBook == null) return NotFound();

            var methods = await _receptionistService.GetPaymentMethodsAsync();

            return View(new ReceptionistBookingCreateViewModel
            {
                RoomId = roomId,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                NumberOfGuests = guests,
                TotalAmount = nights * roomToBook.PricePerNight,
                PaymentMethods = methods
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(ReceptionistBookingCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PaymentMethods = await _receptionistService.GetPaymentMethodsAsync();
                return View(model);
            }

            //var success = await _receptionistService.CreateBookingAsync(model);

            //if (!success)
            //{
            //    ModelState.AddModelError("", "Could not create booking.");
            //    model.PaymentMethods = await _receptionistService.GetPaymentMethodsAsync();
            //    return View(model);
            //}

            //return RedirectToAction("Search");

            var result = await _receptionistService.CreateBookingAsync(model);
            if (result == null)
            {
                ModelState.AddModelError("", "Could not complete booking.");
                model.PaymentMethods = await _receptionistService.GetPaymentMethodsAsync();
                return View(model);
            }

            return View("Success", result);
        }
    }
}
