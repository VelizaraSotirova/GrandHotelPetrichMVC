using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.ViewModels.Admin.Staff;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GrandHotelPetrichMVC.Services.Core.Contracts
{
    public interface IStaffService
    {
        Task<IEnumerable<StaffListViewModel>> GetStaffAsync(string statusFilter);
        Task<bool> ChangeStaffStatusAsync(Guid staffId, string newStatus);
        Task<List<User>> GetEligibleUsersAsync();
        Task<List<SelectListItem>> GetEligibleUsersDropdownAsync();
        Task<bool> CreateStaffAsync(CreateStaffViewModel model);
    }
}
