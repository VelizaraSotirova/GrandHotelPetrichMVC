using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Guests.Booking;
using GrandHotelPetrichMVC.ViewModels.Receptionists.Booking;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GrandHotelPetrichMVC.Services.Core
{
    public class ReceptionistService : IReceptionistService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReceptionistService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<AvailableRoomViewModel>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests, RoomType? roomType)
        {
            var overlapping = await _context.Bookings
                .Where(b => b.CheckInDate < checkOut && b.CheckOutDate > checkIn)
                .Select(b => b.RoomId)
                .ToListAsync();

            var query = _context.Rooms.Where(r => !overlapping.Contains(r.Id) && r.MaxCapacity >= guests);

            if (roomType.HasValue)
                query = query.Where(r => r.RoomType == roomType);

            return await query.Select(r => new AvailableRoomViewModel
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Capacity = r.MaxCapacity,
                PricePerNight = r.PricePerNight,
                RoomTypeName = r.RoomType,
                ImageUrl = r.ImageUrl
            }).ToListAsync();
        }

        public async Task<List<PaymentMethodViewModel>> GetPaymentMethodsAsync()
        {
            return await _context.PaymentMethods
                .Select(pm => new PaymentMethodViewModel
                {
                    Id = pm.Id,
                    Name = pm.Name
                }).ToListAsync();
        }

        //public async Task<bool> CreateBookingAsync(ReceptionistBookingCreateViewModel model)
        //{
        //    var user = await _userManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //    {
        //        user = new User
        //        {
        //            Email = model.Email,
        //            UserName = model.Email,
        //            FirstName = model.FirstName,
        //            LastName = model.LastName,
        //            PhoneNumber = model.PhoneNumber,
        //            EmailConfirmed = true
        //        };

        //        var result = await _userManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            await _userManager.AddToRoleAsync(user, "Customer");
        //        }
        //    }

        //    var booking = new Booking
        //    {
        //        Id = Guid.NewGuid(),
        //        UserId = user.Id,
        //        RoomId = model.RoomId,
        //        CheckInDate = model.CheckInDate,
        //        CheckOutDate = model.CheckOutDate,
        //        NumberOfGuests = model.NumberOfGuests,
        //        PaymentMethodId = model.PaymentMethodId,
        //        TotalAmount = model.TotalAmount,
        //        BookingStatus = BookingStatus.Confirmed,
        //        PaymentStatus = PaymentStatus.Paid,
        //        CreatedAt = DateTime.UtcNow,
        //        UpdatedAt = DateTime.UtcNow
        //    };

        //    _context.Bookings.Add(booking);
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        public async Task<ReceptionistBookingSuccessViewModel?> CreateBookingAsync(ReceptionistBookingCreateViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                user = new User
                {
                    Email = model.Email,
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(user);
                await _userManager.AddToRoleAsync(user, "Customer");
            }

            var room = await _context.Rooms.FindAsync(model.RoomId);
            var nights = (model.CheckOutDate - model.CheckInDate).Days;
            var paymentMethod = await _context.PaymentMethods.FindAsync(model.PaymentMethodId);

            if (room == null || paymentMethod == null) return null;

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RoomId = model.RoomId,
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                NumberOfGuests = model.NumberOfGuests,
                PaymentMethodId = model.PaymentMethodId,
                TotalAmount = room.PricePerNight * nights,
                BookingStatus = BookingStatus.Confirmed,
                PaymentStatus = PaymentStatus.Paid,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return new ReceptionistBookingSuccessViewModel
            {
                GuestFullName = $"{user.FirstName} {user.LastName}",
                GuestEmail = user.Email!,
                RoomName = room.Name,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                TotalAmount = room.PricePerNight * nights,
                PaymentMethod = paymentMethod.Name
            };
        }
    }
}
