using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services.Core;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Guests.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Areas.Guests.Controllers
{
    [Area("Guests")]
    [Authorize(Roles = "Customer")]
    public class BookingController : Controller
    {
        private readonly IBookingService _roomService;
        private readonly UserManager<User> _userManager;

        public BookingController(IBookingService roomService, UserManager<User> userManager)
        {
            _roomService = roomService;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new BookingSearchViewModel
            {
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1)
            });
        }

        [HttpPost]
        public async Task<IActionResult> Index(BookingSearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.AvailableRooms = (List<AvailableRoomViewModel>)await _roomService.GetAvailableRoomsAsync(model.CheckInDate, model.CheckOutDate, model.NumberOfGuests, model.RoomType);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Confirm(Guid roomId, DateTime checkIn, DateTime checkOut, int guests)
        {
            try
            {
                var model = await _roomService.PrepareBookingConfirmationAsync(roomId, checkIn, checkOut, guests);
                return View(model);
            }
            catch
            {
                return NotFound(); // or custom error page
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(BookingConfirmationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PaymentMethods = await _roomService.GetPaymentMethodsAsync(); // optional helper
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var success = await _roomService.ConfirmBookingAsync(model, user.Id);

            if (!success) return BadRequest(); // or custom error page

            TempData["Success"] = "Your booking was confirmed!";
            return RedirectToAction("Index", "Home", new { area = "" });
        }


        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }

    }

}
