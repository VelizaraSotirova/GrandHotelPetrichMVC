using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Guests.Review;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GrandHotelPetrichMVC.Services.Core
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CanUserSubmitReviewAsync(string userId)
        {
            return await _context.Bookings
                .AnyAsync(b => b.UserId == userId && b.CheckOutDate < DateTime.UtcNow);
        }

        public async Task<bool> SubmitReviewAsync(CreateReviewViewModel model, string userId)
        {
            // Ensure user has a completed booking
            var hasCompletedStay = await _context.Bookings
                .AnyAsync(b => b.UserId == userId && b.CheckOutDate < DateTime.UtcNow);

            if (!hasCompletedStay) return false;

            var review = new Review
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoomId = model.RoomId,
                Title = model.Title,
                Comment = model.Comment,
                Rating = model.Rating,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsApproved = false,  // Default: needs approval
                IsFeatured = false
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ReviewDisplayViewModel>> GetApprovedReviewsAsync()
        {
            return await _context.Reviews
                .Where(r => r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .Include(r => r.User)
                .Select(r => new ReviewDisplayViewModel
                {
                    UserFullName = r.User.FirstName + " " + r.User.LastName,
                    Title = r.Title,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt,
                    IsFeatured = r.IsFeatured
                })
                .ToListAsync();
        }
    }
}
