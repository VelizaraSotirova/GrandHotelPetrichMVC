using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services.Core;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Guests.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        [ValidateAntiForgeryToken]
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
        public async Task<IActionResult> SelectRoom(Guid roomId, DateTime checkIn, DateTime checkOut, int guests)
        {
            var room = await _roomService.GetRoomDetailsAsync(roomId);
            if (room == null)
            {
                return NotFound();
            }

            var viewModel = new BookingConfirmationViewModel
            {
                RoomId = roomId,
                RoomType = room.RoomTypeName,
                RoomImageUrl = room.ImageUrl,
                Description = room.Description,
                PricePerNight = room.PricePerNight,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                NumberOfGuests = guests,
                TotalAmount = (decimal)(checkOut - checkIn).TotalDays * room.PricePerNight,
                PaymentMethods = await _roomService.GetPaymentMethodsAsync()
            };

            return View("Confirm", viewModel);
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
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var bookingId = await _roomService.ConfirmBookingAsync(model, user.Id);

            if (bookingId == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Booking could not be created.");
                return View(model);
            }

            return RedirectToAction("Success", new { id = bookingId });
        }

        [HttpGet]
        public async Task<IActionResult> Success(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var viewModel = await _roomService.GetBookingSuccessAsync(id, user.Id);
            if (viewModel == null) return NotFound();

            return View(viewModel);
        }

        public async Task<IActionResult> MyBookings(string filter = "All")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var viewModel = await _roomService.GetBookingsForUserAsync(user.Id, filter);
            return View(viewModel);
        }
    }
}
