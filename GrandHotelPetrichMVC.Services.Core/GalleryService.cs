using global::GrandHotelPetrichMVC.Data;
using global::GrandHotelPetrichMVC.Data.Models;
using global::GrandHotelPetrichMVC.Services.Core.Contracts;
using global::GrandHotelPetrichMVC.ViewModels.Admin.Gallery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GrandHotelPetrichMVC.Services.Core
{
    public class GalleryService : IGalleryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public GalleryService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<List<GalleryImageViewModel>> GetAllImagesAsync()
        {
            return await _context.Galleries
                .Include(g => g.Category)
                .OrderByDescending(g => g.UpdatedAt)
                .Select(g => new GalleryImageViewModel
                {
                    Id = g.Id,
                    ImageUrl = g.ImageUrl!,
                    Description = g.Description
                }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetCategoriesAsync()
        {
            return await _context.GalleryCategories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync();
        }

        public async Task<bool> UploadImageAsync(GalleryUploadViewModel model, string imagePath)
        {
            if (model.ImageFile == null || model.ImageFile.Length == 0)
                return false;

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";
            var uploadPath = Path.Combine(_env.WebRootPath, "images", "gallery");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(stream);
            }

            var category = await _context.GalleryCategories.FindAsync(model.CategoryId);
            if (category == null)
                return false;

            var gallery = new Gallery
            {
                Id = Guid.NewGuid(),
                ImageUrl = $"/images/gallery/{fileName}",
                CategoryId = model.CategoryId,
                Title = category.Name,
                Description = model.Description,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Galleries.Add(gallery);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteImageAsync(Guid id)
        {
            var image = await _context.Galleries.FindAsync(id);
            if (image == null)
                return false;

            var filePath = Path.Combine(_env.WebRootPath, image.ImageUrl!.TrimStart('/'));

            if (File.Exists(filePath))
                File.Delete(filePath);

            _context.Galleries.Remove(image);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}