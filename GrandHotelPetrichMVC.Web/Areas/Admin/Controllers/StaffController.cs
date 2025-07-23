using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Admin.Staff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GrandHotelPetrichMVC.Web.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;
        private readonly UserManager<User> _userManager;

        public StaffController(IStaffService staffService, UserManager<User> userManager)
        {
            _staffService = staffService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string filter = "All")
        {
            var model = await _staffService.GetStaffAsync(filter);
            ViewBag.CurrentFilter = filter;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid staffId, string status)
        {
            var success = await _staffService.ChangeStaffStatusAsync(staffId, status);
            if (!success)
            {
                return BadRequest("Failed to update status.");
            }

            return RedirectToAction(nameof(Index), new { filter = "All" });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var usersWithoutStaff = await _staffService.GetEligibleUsersAsync();
            var model = new CreateStaffViewModel();

            ViewBag.EligibleUsers = usersWithoutStaff
                .Select(u => new SelectListItem
                {
                    Value = u.Email,
                    Text = $"{u.FirstName} {u.LastName} ({u.Email})"
                }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStaffViewModel model)
        {
            if (model.Salary < 600)
            {
                ModelState.AddModelError(nameof(model.Salary), "Salary must be greater than 600.");
            }

            //if (!ModelState.IsValid)
            //{
            //    ViewBag.EligibleUsers = await _staffService.GetEligibleUsersDropdownAsync();
            //    return View(model);
            //}

            var result = await _staffService.CreateStaffAsync(model);
            if (!result)
            {
                ModelState.AddModelError("", "Could not create staff member.");
                ViewBag.EligibleUsers = await _staffService.GetEligibleUsersDropdownAsync();
                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}