using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Admin.Staff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var staff = await _staffService.GetAllStaffAsync();
        //    return View(staff);
        //}

        public async Task<IActionResult> Index(string filter = "All")
        {
            var model = await _staffService.GetAllStaffAsync(filter);
            ViewBag.CurrentFilter = filter;
            return View(model);
        }


        public async Task<IActionResult> ChangeStatus(Guid id, StaffStatus newStatus)
        {
            var result = await _staffService.UpdateStaffStatusAsync(id, newStatus);
            if (!result) 
            { 
                return BadRequest("Failed to update status."); 
            }

            return RedirectToAction(nameof(Index), new { filter = "All" });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateStaffViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStaffViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _staffService.CreateStaffAsync(model);
            if (!success) return BadRequest("Could not create staff");

            return RedirectToAction("Index");
        }
    }
}
