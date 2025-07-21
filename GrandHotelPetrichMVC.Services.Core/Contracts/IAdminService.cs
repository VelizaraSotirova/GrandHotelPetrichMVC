using GrandHotelPetrichMVC.ViewModels.Admin.ContactMessage;
using GrandHotelPetrichMVC.ViewModels.Admin.Dashboard;

namespace GrandHotelPetrichMVC.Services.Core.Contracts
{
    public interface IAdminService
    {
        Task<AdminDashboardViewModel> GetDashboardStatsAsync();
        Task<List<ContactMessageAdminViewModel>> GetAllContactMessagesAsync();
    }
}
