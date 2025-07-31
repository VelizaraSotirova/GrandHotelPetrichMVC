using Microsoft.AspNetCore.Http;

namespace GrandHotelPetrichMVC.Services.Core.Contracts
{
    public interface IFileService
    {
        Task SaveFileAsync(IFormFile file, string fullPath);
        bool FileExists(string fullPath);
        void DeleteFile(string fullPath);
    }

}
