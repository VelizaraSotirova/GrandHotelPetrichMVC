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

        public async Task<IEnumerable<StaffListViewModel>> GetStaffAsync(string filter)
        {
            var staffQuery = _context.Staff.Include(s => s.User).AsQueryable();

            if (filter != "All" && Enum.TryParse<StaffStatus>(filter, out var status))
            {
                staffQuery = staffQuery.Where(s => s.Status == status);
            }

            var staffList = await staffQuery.ToListAsync();
            var result = new List<StaffListViewModel>();

            foreach (var staff in staffList)
            {
                var user = staff.User;
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new StaffListViewModel
                {
                    Id = staff.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber!,
                    Role = roles.FirstOrDefault() ?? "N/A", // fallback if no role
                    Status = staff.Status
                });
            }

            return result;
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

            user.PhoneNumber = model.PhoneNumber;
            await _userManager.UpdateAsync(user);

            // Remove default Customer role if exists
            if (await _userManager.IsInRoleAsync(user, "Customer"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Customer");
            }

            // Assign correct role
            var roleName = model.Role.ToString(); // e.g., "Receptionist"
            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }

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
