using GrandHotelPetrichMVC.Services.Core.Contracts;
using Microsoft.AspNetCore.Http;

namespace GrandHotelPetrichMVC.Services.Core
{
    public class FileService : IFileService
    {
        public async Task SaveFileAsync(IFormFile file, string fullPath)
        {
            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        public bool FileExists(string fullPath)
        {
            return File.Exists(fullPath);
        }

        public void DeleteFile(string fullPath)
        {
            File.Delete(fullPath);
        }
    }
}
