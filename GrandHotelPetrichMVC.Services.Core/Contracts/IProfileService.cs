using GrandHotelPetrichMVC.ViewModels.Guests.Profile;

namespace GrandHotelPetrichMVC.Services.Core.Contracts
{
    public interface IProfileService
    {
        Task<ProfileViewModel?> GetProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, ProfileViewModel model);
    }
}
