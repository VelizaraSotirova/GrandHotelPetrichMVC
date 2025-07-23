using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Admin.Staff;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GrandHotelPetrichMVC.Services.Core
{
    public class StaffService : IStaffService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public StaffService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<StaffListViewModel>> GetStaffAsync(string statusFilter)
        {
            var query = _context.Staff.Include(s => s.User).AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter.ToLower() != "all")
            {
                if (Enum.TryParse<StaffStatus>(statusFilter, true, out var parsedStatus))
                {
                    query = query.Where(s => s.Status == parsedStatus);
                }
            }

            return await query.Select(s => new StaffListViewModel
            {
                Id = s.Id,
                FullName = s.User.FirstName + " " + s.User.LastName,
                Email = s.User.Email!,
                PhoneNumber = s.User.PhoneNumber!,
                Status = s.Status
            }).ToListAsync();
        }

        public async Task<bool> ChangeStaffStatusAsync(Guid staffId, string newStatus)
        {
            var staff = await _context.Staff.FindAsync(staffId);
            if (staff == null) return false;

            if (!Enum.TryParse<StaffStatus>(newStatus, out var statusEnum)) return false;

            staff.Status = statusEnum;
            _context.Staff.Update(staff);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<User>> GetEligibleUsersAsync()
        {
            var staffUserIds = await _context.Staff.Select(s => s.UserId).ToListAsync();
            return await _context.Users
                .Where(u => !staffUserIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetEligibleUsersDropdownAsync()
        {
            var users = await GetEligibleUsersAsync();
            return users.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = $"{u.FirstName} {u.LastName} ({u.Email})"
            }).ToList();
        }

        public async Task<bool> CreateStaffAsync(CreateStaffViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.UserEmail);
            if (user == null) return false;

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Role = model.Role,
                Shift = model.Shift,
                Salary = model.Salary,
                Status = StaffStatus.Active,
                HireDate = DateTime.UtcNow
            };

            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
