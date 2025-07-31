using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core;
using GrandHotelPetrichMVC.ViewModels.Guests.Booking;
using Microsoft.EntityFrameworkCore;

namespace ServiceTests
{
    [TestFixture]
    public class BookingTests
    {
        private ApplicationDbContext _context;
        private BookingService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new BookingService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAvailableRoomsAsync_ReturnsCorrectRooms()
        {
            // Arrange
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Standard",
                MaxCapacity = 2,
                RoomType = RoomType.Standard,
                Description = "A standard room",
                PricePerNight = 100,
                ImageUrl = "/img.jpg"
            };

            var conflictingBooking = new Booking
            {
                Id = Guid.NewGuid(),
                RoomId = room.Id,
                UserId = Guid.NewGuid().ToString(),
                CheckInDate = new DateTime(2025, 7, 10),
                CheckOutDate = new DateTime(2025, 7, 15)
            };

            await _context.Rooms.AddAsync(room);
            await _context.Bookings.AddAsync(conflictingBooking);
            await _context.SaveChangesAsync();

            var checkIn = new DateTime(2025, 7, 12);
            var checkOut = new DateTime(2025, 7, 16);

            // Act
            var result = await _service.GetAvailableRoomsAsync(checkIn, checkOut, 2, RoomType.Standard);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task ConfirmBookingAsync_SavesBookingAndRevenueCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var roomId = Guid.NewGuid();
            var methodId = Guid.NewGuid();
            var sourceId = Guid.NewGuid();

            var room = new Room
            {
                Id = roomId,
                Name = "Suite",
                MaxCapacity = 3,
                PricePerNight = 150,
                ImageUrl = "/suite.jpg",
                RoomType = RoomType.Suite,
                Description = "Big suite"
            };

            var status = new StatusOfRoom { Id = Guid.NewGuid(), RoomId = roomId, Status = RoomStatus.Available };
            var method = new PaymentMethod { Id = methodId, Name = "Credit Card", IsActive = true };
            var revenueSource = new RevenueSource { Id = sourceId, Name = "Room", IsActive = true };

            await _context.Rooms.AddAsync(room);
            await _context.RoomStatuses.AddAsync(status);
            await _context.PaymentMethods.AddAsync(method);
            await _context.RevenueSources.AddAsync(revenueSource);
            await _context.SaveChangesAsync();

            var model = new BookingConfirmationViewModel
            {
                RoomId = roomId,
                CheckInDate = DateTime.UtcNow.Date,
                CheckOutDate = DateTime.UtcNow.AddDays(2).Date,
                NumberOfGuests = 2,
                TotalAmount = 300,
                PaymentMethodId = methodId
            };

            // Act
            var bookingId = await _service.ConfirmBookingAsync(model, userId);

            // Assert
            var booking = await _context.Bookings.FindAsync(bookingId);
            var revenue = await _context.Revenues.FirstOrDefaultAsync(r => r.BookingId == bookingId);
            var statusAfter = await _context.RoomStatuses.FirstAsync(r => r.RoomId == roomId);

            Assert.IsNotNull(booking);
            Assert.That(booking.UserId, Is.EqualTo(userId));
            Assert.IsNotNull(revenue);
            Assert.That(statusAfter.Status, Is.EqualTo(RoomStatus.Occupied));
        }

        [Test]
        public async Task GetBookingsForUserAsync_ReturnsCorrectFilteredBookings()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Deluxe",
                PricePerNight = 120,
                Description = "A deluxe room",
                ImageUrl = "/deluxe.jpg",
            };

            var booking1 = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Room = room,
                CheckInDate = DateTime.UtcNow.AddDays(-3),
                CheckOutDate = DateTime.UtcNow.AddDays(1),
                TotalAmount = 300,
                BookingStatus = BookingStatus.Confirmed
            };

            var booking2 = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Room = room,
                CheckInDate = DateTime.UtcNow.AddDays(-10),
                CheckOutDate = DateTime.UtcNow.AddDays(-5),
                TotalAmount = 300,
                BookingStatus = BookingStatus.Confirmed
            };

            await _context.Rooms.AddAsync(room);
            await _context.Bookings.AddRangeAsync(booking1, booking2);
            await _context.SaveChangesAsync();

            // Act
            var activeBookings = await _service.GetBookingsForUserAsync(userId, "active");
            var passedBookings = await _service.GetBookingsForUserAsync(userId, "passed");

            // Assert
            Assert.That(activeBookings.Bookings.Count, Is.EqualTo(1));
            Assert.That(passedBookings.Bookings.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetBookingSuccessAsync_ReturnsNullIfNotFound()
        {
            var result = await _service.GetBookingSuccessAsync(Guid.NewGuid(), Guid.NewGuid().ToString());
            Assert.IsNull(result);
        }
    }
}
