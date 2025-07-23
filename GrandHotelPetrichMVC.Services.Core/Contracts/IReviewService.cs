using GrandHotelPetrichMVC.ViewModels.Guests.Review;

namespace GrandHotelPetrichMVC.Services.Core.Contracts
{
    public interface IReviewService
    {
        Task<bool> CanUserSubmitReviewAsync(string userId);
        Task<bool> SubmitReviewAsync(CreateReviewViewModel model, string userId);
        Task<List<ReviewDisplayViewModel>> GetApprovedReviewsAsync();
    }
}
