using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Admin.ContactMessage;
using GrandHotelPetrichMVC.ViewModels.Admin.Dashboard;
using Microsoft.AspNetCore.Identity;
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
                StaffCount = await _context.Staff.CountAsync(),
                MonthlyRevenue = await _context.Revenues
                                    .Where(r => r.Date >= startOfMonth)
                                    .SumAsync(r => (decimal?)r.Amount) ?? 0,
                PaidSalaries = await _context.Staff
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

    }
}
