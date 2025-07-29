using GrandHotelPetrichMVC.ViewModels.Admin.Amenity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GrandHotelPetrichMVC.Services.Core.Contracts
{
    public interface IAmenityService
    {
        Task<List<AmenityViewModel>> GetAllAsync();
        Task<bool> AddAsync(AddAmenityViewModel model);
        Task<bool> DeactivateAsync(Guid id);
        Task<bool> ActivateAsync(Guid id);
    }
}
