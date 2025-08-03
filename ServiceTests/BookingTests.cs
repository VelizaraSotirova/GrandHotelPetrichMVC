using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core;
using GrandHotelPetrichMVC.Services.Core.Contracts;
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
        public async Task ConfirmBookingAsync_SavesSpecialRequestsCorrectly()
        {
            // Arrange
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room A",
                PricePerNight = 100,
                ImageUrl = "/suite.jpg",
                Description = "Big suite",
                MaxCapacity = 2
            };

            var paymentMethod = new PaymentMethod
            {
                Id = Guid.NewGuid(),
                Name = "Credit Card",
                IsActive = true
            };

            var revenueSource = new RevenueSource
            {
                Id = Guid.NewGuid(),
                Name = "Room"
            };

            var roomStatus = new StatusOfRoom
            {
                RoomId = room.Id,
                Status = RoomStatus.Available,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Rooms.AddAsync(room);
            await _context.PaymentMethods.AddAsync(paymentMethod);
            await _context.RevenueSources.AddAsync(revenueSource);
            await _context.RoomStatuses.AddAsync(roomStatus);
            await _context.SaveChangesAsync();

            var viewModel = new BookingConfirmationViewModel
            {
                RoomId = room.Id,
                CheckInDate = DateTime.UtcNow.Date.AddDays(1),
                CheckOutDate = DateTime.UtcNow.Date.AddDays(3),
                NumberOfGuests = 2,
                PaymentMethodId = paymentMethod.Id,
                TotalAmount = 200,
                SpecialRequests = "I want a crib and a sea view." // ✅ this is what we test
            };

            var userId = "test-user-id";

            // Act
            var bookingId = await _service.ConfirmBookingAsync(viewModel, userId);

            // Assert
            var booking = await _context.Bookings.FindAsync(bookingId);

            Assert.That(booking, Is.Not.Null);
            Assert.That(booking!.SpecialRequests, Is.EqualTo("I want a crib and a sea view."));
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

        [Test]
        public async Task GetRoomDetailsAsync_ReturnsRoomDetails_WhenExists()
        {
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Eco Room",
                PricePerNight = 90,
                MaxCapacity = 2,
                RoomType = RoomType.BusinessRoom,
                Description = "Eco desc",
                ImageUrl = "/eco.jpg"
            };
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            var result = await _service.GetRoomDetailsAsync(room.Id);

            Assert.IsNotNull(result);
            Assert.That(result!.Name, Is.EqualTo("Eco Room"));
        }

        [Test]
        public async Task GetRoomDetailsAsync_ReturnsNull_WhenRoomNotFound()
        {
            var result = await _service.GetRoomDetailsAsync(Guid.NewGuid());
            Assert.IsNull(result);
        }

        [Test]
        public void PrepareBookingConfirmationAsync_Throws_WhenRoomNotFound()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(() =>
                _service.PrepareBookingConfirmationAsync(Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 2));

            Assert.That(ex!.Message, Does.Contain("Invalid Room ID"));
        }

        [Test]
        public async Task PrepareBookingConfirmationAsync_ReturnsConfirmationWithTotal()
        {
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Premium",
                PricePerNight = 200,
                MaxCapacity = 2,
                RoomType = RoomType.Suite,
                Description = "Premium room",
                ImageUrl = "/premium.jpg"
            };
            var paymentMethod = new PaymentMethod { Id = Guid.NewGuid(), Name = "Card" };

            await _context.Rooms.AddAsync(room);
            await _context.PaymentMethods.AddAsync(paymentMethod);
            await _context.SaveChangesAsync();

            var checkIn = DateTime.UtcNow;
            var checkOut = checkIn.AddDays(3);

            var result = await _service.PrepareBookingConfirmationAsync(room.Id, checkIn, checkOut, 2);

            Assert.That(result.TotalAmount, Is.EqualTo(600));
            Assert.That(result.PaymentMethods.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetBookingConfirmationAsync_ReturnsViewModel()
        {
            // Arrange
            var roomId = Guid.NewGuid();
            var paymentMethod = new PaymentMethod
            {
                Id = Guid.NewGuid(),
                Name = "Card",
                IsActive = true
            };

            var room = new Room
            {
                Id = roomId,
                Name = "Executive Suite",
                Description = "Spacious and elegant",
                RoomType = RoomType.Suite,
                MaxCapacity = 3,
                PricePerNight = 200m,
                ImageUrl = "/img/exec.jpg"
            };

            await _context.Rooms.AddAsync(room);
            await _context.PaymentMethods.AddAsync(paymentMethod);
            await _context.SaveChangesAsync();

            var checkIn = DateTime.UtcNow.Date;
            var checkOut = checkIn.AddDays(2);

            // Act
            var result = await _service.GetBookingConfirmationAsync(roomId, checkIn, checkOut, 2);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RoomId, Is.EqualTo(roomId));
            Assert.That(result.RoomName, Is.EqualTo("Executive Suite"));
            Assert.That(result.RoomImageUrl, Is.EqualTo("/img/exec.jpg"));
            Assert.That(result.RoomType, Is.EqualTo(RoomType.Suite));
            Assert.That(result.TotalAmount, Is.EqualTo(2 * 200m));
            Assert.That(result.PaymentMethods.Count, Is.EqualTo(1));
            Assert.That(result.PaymentMethods.First().Name, Is.EqualTo("Card"));
        }


        [Test]
        public void GetBookingConfirmationAsync_Throws_WhenRoomNotFound()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(() =>
                _service.GetBookingConfirmationAsync(Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 2));

            Assert.That(ex!.Message, Does.Contain("Invalid room"));
        }

        [Test]
        public async Task GetPaymentMethodsAsync_ReturnsAllMethods()
        {
            await _context.PaymentMethods.AddRangeAsync(
                new PaymentMethod { Id = Guid.NewGuid(), Name = "Cash" },
                new PaymentMethod { Id = Guid.NewGuid(), Name = "Bank Transfer" }
            );
            await _context.SaveChangesAsync();

            var result = await _service.GetPaymentMethodsAsync();

            Assert.That(result.Count, Is.EqualTo(2));
        }
    }
}
