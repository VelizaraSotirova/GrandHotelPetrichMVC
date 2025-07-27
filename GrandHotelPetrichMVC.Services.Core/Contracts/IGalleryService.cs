using GrandHotelPetrichMVC.ViewModels.Admin.Gallery;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GrandHotelPetrichMVC.Services.Core.Contracts
{
    public interface IGalleryService
    {
        Task<List<GalleryImageViewModel>> GetAllImagesAsync();
        Task<List<SelectListItem>> GetCategoriesAsync();
        Task<bool> UploadImageAsync(GalleryUploadViewModel model, string imagePath);
        Task<bool> DeleteImageAsync(Guid id);
    }
}
