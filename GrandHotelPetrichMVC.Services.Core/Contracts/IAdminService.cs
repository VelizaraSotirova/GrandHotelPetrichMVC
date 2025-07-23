using GrandHotelPetrichMVC.ViewModels.Admin.ContactMessage;
using GrandHotelPetrichMVC.ViewModels.Admin.Dashboard;
using GrandHotelPetrichMVC.ViewModels.Admin.Review;

namespace GrandHotelPetrichMVC.Services.Core.Contracts
{
    public interface IAdminService
    {
        Task<AdminDashboardViewModel> GetDashboardStatsAsync();
        Task<List<ContactMessageAdminViewModel>> GetAllContactMessagesAsync();


        Task<List<ReviewAdminViewModel>> GetAllReviewsAsync();
        Task<bool> ApproveReviewAsync(Guid reviewId);

    }
}
