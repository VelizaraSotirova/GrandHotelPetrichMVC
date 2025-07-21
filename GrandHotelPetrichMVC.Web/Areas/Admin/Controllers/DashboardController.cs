using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Admin.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GrandHotelPetrichMVC.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var availableRooms = await _context.RoomStatuses
                .CountAsync(rs => rs.Status == GCommon.Enums.RoomStatus.Available);

            var occupiedRooms = await _context.RoomStatuses
                .CountAsync(rs => rs.Status == GCommon.Enums.RoomStatus.Occupied);

            var staffCount = await _context.Staff.CountAsync();

            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");
            var registeredUsersCount = userRole != null
                ? await _context.UserRoles.CountAsync(ur => ur.RoleId == userRole.Id)
                : 0;

            var monthlyRevenue = await _context.Revenues
                .Where(r => r.Date >= startOfMonth)
                .SumAsync(r => (decimal?)r.Amount) ?? 0;

            var paidSalaries = await _context.Staff
                .Where(s => s.Status == GCommon.Enums.StaffStatus.Active)
                .SumAsync(s => (decimal?)s.Salary) ?? 0;

            var model = new AdminDashboardViewModel
            {
                AvailableRooms = availableRooms,
                OccupiedRooms = occupiedRooms,
                StaffCount = staffCount,
                MonthlyRevenue = monthlyRevenue,
                PaidSalaries = paidSalaries
            };

            return View(model);
        }
    }
}
