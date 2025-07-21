using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Admin.Staff;
using Microsoft.EntityFrameworkCore;

namespace GrandHotelPetrichMVC.Services.Core
{
    public class StaffService : IStaffService
    {
        private readonly ApplicationDbContext _context;

        public StaffService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StaffListViewModel>> GetAllStaffAsync(string statusFilter)
        {
            var query = _context.Staff.Include(s => s.User).AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter.ToLower() != "all")
            {
                if (Enum.TryParse<StaffStatus>(statusFilter, true, out var parsedStatus))
                {
                    query = query.Where(s => s.Status == parsedStatus);
                }
            }

            return await _context.Staff
                .Include(s => s.User)
                .Select(s => new StaffListViewModel
                {
                    Id = s.Id,
                    FullName = s.User.FirstName + " " + s.User.LastName,
                    Email = s.User.Email!,
                    Role = s.Role,
                    Shift = s.Shift,
                    Status = s.Status
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateStaffStatusAsync(Guid staffId, StaffStatus newStatus)
        {
            var staff = await _context.Staff.FindAsync(staffId);
            if (staff == null) return false;

            staff.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateStaffAsync(CreateStaffViewModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.UserEmail);
            if (user == null) return false;

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Role = model.Role,
                Shift = model.Shift,
                Status = StaffStatus.Active,
                Salary = model.Salary,
                HireDate = DateTime.UtcNow
            };

            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
