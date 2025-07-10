using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Guests.Booking;
using Microsoft.EntityFrameworkCore;

namespace GrandHotelPetrichMVC.Services.Core
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
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

        public async Task<BookingConfirmationViewModel> PrepareBookingConfirmationAsync(Guid roomId, DateTime checkIn, DateTime checkOut, int guests)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
                throw new ArgumentException("Invalid Room ID");

            var days = (checkOut - checkIn).Days;
            var total = room.PricePerNight * days;

            var paymentMethods = await _context.PaymentMethods
                .Select(pm => new PaymentMethodViewModel
                {
                    Id = pm.Id,
                    Name = pm.Name
                })
                .ToListAsync();

            return new BookingConfirmationViewModel
            {
                RoomId = roomId,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                NumberOfGuests = guests,
                TotalAmount = total,
                PaymentMethods = paymentMethods
            };
        }

        public async Task<bool> ConfirmBookingAsync(BookingConfirmationViewModel model, string userId)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == model.RoomId);
            if (room == null) return false;

            var days = (model.CheckOutDate - model.CheckInDate).Days;
            var total = room.PricePerNight * days;

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoomId = model.RoomId,
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                NumberOfGuests = model.NumberOfGuests,
                TotalAmount = total,
                SpecialRequests = model.SpecialRequests,
                BookingStatus = BookingStatus.Confirmed,
                PaymentStatus = PaymentStatus.Paid,
                PaymentMethodId = model.PaymentMethodId
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<List<PaymentMethodViewModel>> GetPaymentMethodsAsync()
        {
            return await _context.PaymentMethods
                .Select(pm => new PaymentMethodViewModel
                {
                    Id = pm.Id,
                    Name = pm.Name
                })
                .ToListAsync();
        }

        public async Task CreateBookingAsync(Booking booking)
        {
            // Mark the room as occupied
            var room = await _context.Rooms.FindAsync(booking.RoomId);
            if (room != null)
            {
                room.IsActive = true;
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

    }
}
