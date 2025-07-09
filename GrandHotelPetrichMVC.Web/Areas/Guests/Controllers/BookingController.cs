using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Guests.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Areas.Guests.Controllers
{
    [Area("Guests")]
    [Authorize(Roles = "Customer")]
    public class BookingController : Controller
    {
        private readonly IRoomService _roomService;

        public BookingController(IRoomService roomService)
        {
            _roomService = roomService;
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
    }

}
