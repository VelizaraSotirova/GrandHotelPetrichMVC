using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Guests.Profile;
using Microsoft.AspNetCore.Identity;

namespace GrandHotelPetrichMVC.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<User> _userManager;

        public ProfileService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ProfileViewModel?> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return null;

            return new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Email = user.Email!
            };
        }

        public async Task<bool> UpdateProfileAsync(string userId, ProfileViewModel model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
