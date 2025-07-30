using GrandHotelPetrichMVC.Services.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin")]
    public class ReviewsController : Controller
    {
        private readonly IAdminService _adminService;

        public ReviewsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            var reviews = await _adminService.GetAllReviewsAsync();
            return View(reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _adminService.ApproveReviewAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}
