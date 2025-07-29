using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Admin.Amenity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GrandHotelPetrichMVC.Services.Core
{
    public class AmenityService : IAmenityService
    {
        private readonly ApplicationDbContext _context;

        public AmenityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AmenityViewModel>> GetAllAsync()
        {
            return await _context.Amenities
                .Select(a => new AmenityViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    IsActive = a.IsActive
                }).ToListAsync();
        }

        public async Task<bool> AddAsync(AddAmenityViewModel model)
        {
            if (await _context.Amenities.AnyAsync(a => a.Name.ToLower() == model.Name.ToLower()))
                return false;

            var amenity = new Amenity
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                IsActive = true
            };

            _context.Amenities.Add(amenity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateAsync(Guid id)
        {
            var amenity = await _context.Amenities.FindAsync(id);
            if (amenity == null) return false;

            amenity.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateAsync(Guid id)
        {
            var amenity = await _context.Amenities.FindAsync(id);
            if (amenity == null) return false;

            amenity.IsActive = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
