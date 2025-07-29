using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Admin.Room;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GrandHotelPetrichMVC.Services.Core
{
    public class RoomAdminService : IRoomAdminService
    {
        private readonly ApplicationDbContext _context;

        public RoomAdminService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<List<RoomListViewModel>> GetAllRoomsAsync()
        {
            return await _context.Rooms
                .Include(r => r.Bookings)
                .Select(r => new RoomListViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    RoomType = r.RoomType,
                    PricePerNight = r.PricePerNight,
                    MaxCapacity = r.MaxCapacity,
                    Badge = r.Badge,
                    ImageUrl = r.ImageUrl,
                    IsActive = r.IsActive,
                    Status = _context.RoomStatuses
                        .Where(rs => rs.RoomId == r.Id)
                        .Select(rs => rs.Status)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<RoomCreateViewModel?> GetRoomByIdAsync(Guid id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return null;

            return new RoomCreateViewModel
            {
                Name = room.Name,
                RoomType = room.RoomType,
                PricePerNight = room.PricePerNight,
                MaxCapacity = room.MaxCapacity,
                BedConfiguration = room.BedConfiguration,
                Description = room.Description,
                ImageUrl = room.ImageUrl,
                Badge = room.Badge
            };
        }

        public async Task<bool> CreateRoomAsync(RoomCreateViewModel model)
        {
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                RoomType = model.RoomType,
                PricePerNight = model.PricePerNight,
                MaxCapacity = model.MaxCapacity,
                BedConfiguration = model.BedConfiguration,
                Description = model.Description ?? "",
                ImageUrl = model.ImageUrl ?? "",
                Badge = model.Badge,
                IsActive = true
            };

            _context.Rooms.Add(room);
            _context.RoomStatuses.Add(new StatusOfRoom
            {
                RoomId = room.Id,
                Status = RoomStatus.Available,
                LastCleaned = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            // Add amenities
            foreach (var amenityId in model.SelectedAmenityIds)
            {
                room.RoomAmenities.Add(new RoomAmenity
                {
                    RoomId = room.Id,
                    AmenityId = amenityId
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRoomAsync(RoomCreateViewModel model)
        {
            var room = await _context.Rooms.FindAsync(model.Id);
            if (room == null) return false;

            room.Name = model.Name;
            room.RoomType = model.RoomType;
            room.PricePerNight = model.PricePerNight;
            room.MaxCapacity = model.MaxCapacity;
            room.BedConfiguration = model.BedConfiguration;
            room.Description = model.Description ?? "";
            room.ImageUrl = model.ImageUrl ?? "";
            room.Badge = model.Badge;
            room.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeRoomStatusAsync(Guid roomId, RoomStatus newStatus)
        {
            var statusRecord = await _context.RoomStatuses.FirstOrDefaultAsync(r => r.RoomId == roomId);
            if (statusRecord == null) return false;

            statusRecord.Status = newStatus;
            statusRecord.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleRoomActiveAsync(Guid roomId, bool isActive)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) return false;

            room.IsActive = isActive;
            room.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<RoomCreateViewModel> GetRoomCreateViewModelAsync()
        {
            var amenities = await _context.Amenities
                .Where(a => a.IsActive)
                .ToListAsync();

            return new RoomCreateViewModel
            {
                AvailableAmenities = amenities
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList()
            };
        }

        public async Task<List<SelectListItem>> GetAmenitiesSelectListAsync()
        {
            return await _context.Amenities
                .Where(a => a.IsActive)
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                })
                .ToListAsync();
        }
    }
}
