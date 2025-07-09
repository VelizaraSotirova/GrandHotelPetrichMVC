using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Guests.Booking;
using Microsoft.EntityFrameworkCore;

namespace GrandHotelPetrichMVC.Services.Core
{
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;

        public RoomService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AvailableRoomViewModel>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests, RoomType? roomType)
        {
            var overlappingBookings = await _context.Bookings
                .Where(b => b.CheckInDate < checkOut && b.CheckOutDate > checkIn)
                .Select(b => b.RoomId)
                .ToListAsync();

            var query = _context.Rooms
                .Where(r => !overlappingBookings.Contains(r.Id) && r.MaxCapacity >= guests);

            if (roomType.HasValue)
            {
                query = query.Where(r => r.RoomType == roomType);
            }

            return await query
                .Select(r => new AvailableRoomViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Capacity = r.MaxCapacity,
                    PricePerNight = r.PricePerNight,
                    RoomTypeName = r.RoomType,
                    ImageUrl = r.ImageUrl
                })
                .ToListAsync();
        }

        public async Task<RoomDetailsViewModel?> GetRoomDetailsAsync(Guid roomId)
        {
            return await _context.Rooms
                .Where(r => r.Id == roomId)
                .Select(r => new RoomDetailsViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    PricePerNight = r.PricePerNight,
                    Capacity = r.MaxCapacity,
                    RoomTypeName = r.RoomType,
                    ImageUrl = r.ImageUrl
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> MarkRoomAsOccupiedAsync(Guid roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) return false;

            room.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
