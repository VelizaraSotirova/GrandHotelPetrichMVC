using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.ViewModels.Admin.Staff;

namespace GrandHotelPetrichMVC.Services.Core.Contracts
{
    public interface IStaffService
    {
        Task<List<StaffListViewModel>> GetAllStaffAsync(string statusFilter);
        Task<bool> UpdateStaffStatusAsync(Guid staffId, StaffStatus newStatus);
        Task<bool> CreateStaffAsync(CreateStaffViewModel model);
    }

}
