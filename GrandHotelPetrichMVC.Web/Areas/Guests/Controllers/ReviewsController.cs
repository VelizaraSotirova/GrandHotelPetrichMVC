using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Guests.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Areas.Guests.Controllers
{
    [Area(nameof(Guests))]
    [Authorize(Roles = "Customer")]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<User> _userManager;

        public ReviewsController(IReviewService reviewService, UserManager<User> userManager)
        {
            _reviewService = reviewService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var reviews = await _reviewService.GetApprovedReviewsAsync();
            return View(reviews);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            var canReview = await _reviewService.CanUserSubmitReviewAsync(userId!);
            if (!canReview)
            {
                TempData["Error"] = "You must complete a stay before writing a review.";
                return RedirectToAction("Index");
            }

            return View(new CreateReviewViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReviewViewModel model)
        {
            var userId = _userManager.GetUserId(User)!;

            if (!ModelState.IsValid)
                return View(model);

            var result = await _reviewService.SubmitReviewAsync(model, userId);
            if (!result)
            {
                TempData["Error"] = "You're not eligible to submit a review.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Review submitted and awaiting approval!";
            
            return RedirectToAction("Index", "Reviews", new { area = "" });
        }
    }

}
