using GrandHotelPetrichMVC.Services.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Areas.Receptionists.Controllers
{
    [Area(nameof(Receptionists))]
    public class RoomsController : Controller
    {
        private readonly IReceptionistService _service;

        public RoomsController(IReceptionistService service)
        {
            _service = service;
        }

        public async Task<IActionResult> OutForCleaning()
        {
            await _service.UpdateRoomsThatNeedCleaningAsync();

            var rooms = await _service.GetRoomsOutForCleaningAsync();
            return View(rooms);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsCleaned(Guid roomId)
        {
            var success = await _service.MarkRoomAsCleanedAsync(roomId);
            if (!success) return BadRequest("Failed to update status.");
            return RedirectToAction("OutForCleaning");
        }
    }
}
