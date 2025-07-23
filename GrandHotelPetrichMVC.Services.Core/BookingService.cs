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

        public async Task<BookingConfirmationViewModel> GetBookingConfirmationAsync(Guid roomId, DateTime checkIn, DateTime checkOut, int guests)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null) throw new ArgumentException("Invalid room");

            var paymentMethods = await _context.PaymentMethods
                .Where(p => p.IsActive)
                .Select(p => new PaymentMethodViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToListAsync();

            var nights = (checkOut - checkIn).Days;
            var total = nights * room.PricePerNight;

            return new BookingConfirmationViewModel
            {
                RoomId = room.Id,
                RoomName = room.Name,
                RoomImageUrl = room.ImageUrl,
                RoomType = room.RoomType,
                PricePerNight = room.PricePerNight,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                NumberOfGuests = guests,
                TotalAmount = total,
                PaymentMethods = paymentMethods
            };
        }

        public async Task<Guid> ConfirmBookingAsync(BookingConfirmationViewModel model, string userId)
        {
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoomId = model.RoomId,
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                NumberOfGuests = model.NumberOfGuests,
                PaymentMethodId = model.PaymentMethodId,
                TotalAmount = model.TotalAmount,
                BookingStatus = BookingStatus.Confirmed,
                PaymentStatus = PaymentStatus.Paid,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);

            var roomRevenueSource = await _context.RevenueSources
                .FirstOrDefaultAsync(rs => rs.Name == "Room");

            if (roomRevenueSource == null)
            {
                throw new Exception("Revenue source 'Room' is missing.");
            }

            var revenue = new Revenue
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                RevenueSourceId = roomRevenueSource.Id,
                Amount = booking.TotalAmount,
                Date = DateTime.UtcNow,
                PaymentMethodId = booking.PaymentMethodId,
                Description = $"Room booking from {booking.CheckInDate:MMM dd} to {booking.CheckOutDate:MMM dd} for {booking.NumberOfGuests} guests."
            };

            _context.Revenues.Add(revenue);

            // Changing status of the room to occupied
            var roomStatus = await _context.RoomStatuses.FirstOrDefaultAsync(s => s.RoomId == booking.RoomId);
            if (roomStatus != null)
            {
                roomStatus.Status = RoomStatus.Occupied;
                roomStatus.UpdatedAt = DateTime.UtcNow;
            }


            await _context.SaveChangesAsync();
            return booking.Id;
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

        public async Task<BookingSuccessViewModel?> GetBookingSuccessAsync(Guid bookingId, string userId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.PaymentMethod)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null) return null;

            return new BookingSuccessViewModel
            {
                BookingId = booking.Id,
                RoomName = booking.Room.Name,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                TotalAmount = booking.TotalAmount,
                PaymentMethod = booking.PaymentMethod?.Name ?? "N/A",
                BookingStatus = booking.BookingStatus.ToString()
            };
        }

        public async Task<MyBookingsViewModel> GetBookingsForUserAsync(string userId, string filter)
        {
            var query = _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Room)
                .AsQueryable();

            var today = DateTime.UtcNow;

            filter = filter.ToLower();
            if (filter == "active")
                query = query.Where(b => b.CheckOutDate >= today);
            else if (filter == "passed")
                query = query.Where(b => b.CheckOutDate < today);

            var bookings = await query
                .OrderByDescending(b => b.CheckInDate)
                .Select(b => new BookingDisplayViewModel
                {
                    Id = b.Id,
                    RoomName = b.Room.Name,
                    ImageUrl = b.Room.ImageUrl,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    TotalAmount = b.TotalAmount,
                    BookingStatus = b.BookingStatus
                })
                .ToListAsync();

            return new MyBookingsViewModel
            {
                Filter = filter,
                Bookings = bookings
            };
        }
    }
}
