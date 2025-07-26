using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Admin.ContactMessage;
using GrandHotelPetrichMVC.ViewModels.Admin.Dashboard;
using GrandHotelPetrichMVC.ViewModels.Admin.Review;
using Microsoft.EntityFrameworkCore;

namespace GrandHotelPetrichMVC.Services.Core
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardViewModel> GetDashboardStatsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            return new AdminDashboardViewModel
            {
                AvailableRooms = await _context.RoomStatuses.CountAsync(r => r.Status == RoomStatus.Available),
                OccupiedRooms = await _context.RoomStatuses.CountAsync(r => r.Status == RoomStatus.Occupied),
                TotalRooms = await _context.Rooms.CountAsync(),
                StaffCount = await _context.Staff
                                .Where(s => s.Status != StaffStatus.Terminated)
                                .CountAsync(),
                MonthlyRevenue = await _context.Revenues
                                    .Where(r => r.Date >= startOfMonth)
                                    .SumAsync(r => (decimal?)r.Amount) ?? 0,
                PaidSalaries = await _context.Staff
                                    .Where(s => s.Status == StaffStatus.Active || s.Status == StaffStatus.OnLeave)
                                    .SumAsync(s => (decimal?)s.Salary) ?? 0 // Assuming salaries are paid monthly
            };
        }

        public async Task<List<ContactMessageAdminViewModel>> GetAllContactMessagesAsync()
        {
            return await _context.ContactMessages
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new ContactMessageAdminViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Email = m.Email,
                    Subject = m.Subject,
                    Message = m.Message,
                    Status = m.Status.ToString(),
                    ReceivedAt = m.CreatedAt
                })
                .ToListAsync();
        }



        public async Task<List<ReviewAdminViewModel>> GetAllReviewsAsync()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Room)
                .Select(r => new ReviewAdminViewModel
                {
                    Id = r.Id,
                    UserFullName = r.User.FirstName + " " + r.User.LastName,
                    Email = r.User.Email!,
                    RoomName = r.Room != null ? r.Room.Name : "General",
                    Rating = r.Rating,
                    Title = r.Title,
                    Comment = r.Comment,
                    IsApproved = r.IsApproved,
                    CreatedAt = r.CreatedAt
                })
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ApproveReviewAsync(Guid reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            review.IsApproved = true;
            review.UpdatedAt = DateTime.UtcNow;
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
