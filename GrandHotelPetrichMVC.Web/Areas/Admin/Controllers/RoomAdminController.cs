using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Admin.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin")]
    public class RoomAdminController : Controller
    {
        private readonly IRoomAdminService _roomService;

        public RoomAdminController(IRoomAdminService roomService)
        {
            _roomService = roomService;
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            return View(rooms);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _roomService.GetRoomCreateViewModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableAmenities = await _roomService.GetAmenitiesSelectListAsync();
                return View(model);
            }

            var success = await _roomService.CreateRoomAsync(model);
            if (!success)
            {
                ModelState.AddModelError("", "Could not create room.");
                model.AvailableAmenities = await _roomService.GetAmenitiesSelectListAsync();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _roomService.GetRoomByIdAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoomCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _roomService.UpdateRoomAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(Guid id, bool isActive)
        {
            await _roomService.ToggleRoomActiveAsync(id, isActive);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(Guid id, RoomStatus status)
        {
            await _roomService.ChangeRoomStatusAsync(id, status);
            return RedirectToAction(nameof(Index));
        }
    }
}
