using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core;
using GrandHotelPetrichMVC.ViewModels.Receptionists.Booking;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ServiceTests
{
    [TestFixture]
    public class ReceptionistTests
    {
        private ApplicationDbContext _context;
        private UserManager<User> _userManager;
        private ReceptionistService _service;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            _context = new ApplicationDbContext(options);

            // Setup fake UserManager
            var store = new Mock<IUserStore<User>>();
            _userManager = MockUserManager.CreateMock(store.Object);

            _service = new ReceptionistService(_context, _userManager);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _userManager.Dispose();
        }

        [Test]
        public async Task GetAvailableRoomsAsync_ReturnsFilteredRooms()
        {
            // Arrange
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room A",
                MaxCapacity = 4,
                PricePerNight = 100,
                RoomType = RoomType.Deluxe,
                Description = "Test Desc",
                ImageUrl = "image.jpg"
            };
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetAvailableRoomsAsync(DateTime.Today, DateTime.Today.AddDays(1), 2, RoomType.Deluxe);

            // Assert
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Name, Is.EqualTo("Room A"));
        }

        [Test]
        public async Task CreateBookingAsync_CreatesBookingForNewGuest()
        {
            // Arrange
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Deluxe Room",
                PricePerNight = 150,
                Description = "Nice room",
                MaxCapacity = 2,
                ImageUrl = "/img.jpg",
                RoomType = RoomType.Standard
            };

            var method = new PaymentMethod
            {
                Id = Guid.NewGuid(),
                Name = "Card",
                IsActive = true
            };

            var source = new RevenueSource
            {
                Id = Guid.NewGuid(),
                Name = "Room",
                IsActive = true
            };

            var status = new StatusOfRoom
            {
                Id = Guid.NewGuid(),
                RoomId = room.Id,
                Status = RoomStatus.Available
            };

            await _context.Rooms.AddAsync(room);
            await _context.PaymentMethods.AddAsync(method);
            await _context.RevenueSources.AddAsync(source);
            await _context.RoomStatuses.AddAsync(status);
            await _context.SaveChangesAsync();

            var model = new ReceptionistBookingCreateViewModel
            {
                Email = "newguest@example.com",
                FirstName = "Alice",
                LastName = "Johnson",
                RoomId = room.Id,
                CheckInDate = DateTime.UtcNow.Date,
                CheckOutDate = DateTime.UtcNow.Date.AddDays(2),
                NumberOfGuests = 1,
                PaymentMethodId = method.Id
            };

            // Act
            var result = await _service.CreateBookingAsync(model);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.GuestEmail, Is.EqualTo("newguest@example.com"));

            var bookings = await _context.Bookings.ToListAsync();
            Assert.That(bookings.Count, Is.EqualTo(1));
            Assert.That(bookings[0].TotalAmount, Is.EqualTo(300)); // 150 x 2 nights
        }


        [Test]
        public async Task CreateBookingAsync_ReturnsNull_WhenRoomNotFound()
        {
            var payment = new PaymentMethod
            {
                Id = Guid.NewGuid(),
                Name = "Cash",
                IsActive = true
            };

            await _context.PaymentMethods.AddAsync(payment);
            await _context.SaveChangesAsync();

            var model = new ReceptionistBookingCreateViewModel
            {
                Email = "guest@example.com",
                FirstName = "Missing",
                LastName = "Room",
                RoomId = Guid.NewGuid(), // Not in DB
                CheckInDate = DateTime.UtcNow,
                CheckOutDate = DateTime.UtcNow.AddDays(1),
                NumberOfGuests = 1,
                PaymentMethodId = payment.Id
            };

            var result = await _service.CreateBookingAsync(model);

            Assert.That(result, Is.Null);
        }


        [Test]
        public async Task CreateBookingAsync_ReturnsNull_WhenPaymentMethodMissing()
        {
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Business",
                PricePerNight = 200,
                Description = "Business room",
                MaxCapacity = 2,
                ImageUrl = "/img.png",
                RoomType = RoomType.BusinessRoom
            };

            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            var model = new ReceptionistBookingCreateViewModel
            {
                Email = "guest@nowhere.com",
                FirstName = "No",
                LastName = "Payment",
                RoomId = room.Id,
                CheckInDate = DateTime.UtcNow,
                CheckOutDate = DateTime.UtcNow.AddDays(1),
                NumberOfGuests = 1,
                PaymentMethodId = Guid.NewGuid() // not in DB
            };

            var result = await _service.CreateBookingAsync(model);

            Assert.That(result, Is.Null);
        }


        [Test]
        public async Task CreateBookingAsync_SetsRoomStatusToOccupied()
        {
            var today = DateTime.UtcNow.Date;

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Suite",
                PricePerNight = 180,
                Description = "Fancy suite",
                MaxCapacity = 2,
                ImageUrl = "/suite.png",
                RoomType = RoomType.Suite
            };

            var method = new PaymentMethod
            {
                Id = Guid.NewGuid(),
                Name = "Paypal"
            };

            var status = new StatusOfRoom
            {
                Id = Guid.NewGuid(),
                RoomId = room.Id,
                Status = RoomStatus.Available
            };

            var revenue = new RevenueSource
            {
                Id = Guid.NewGuid(),
                Name = "Room"
            };

            await _context.Rooms.AddAsync(room);
            await _context.PaymentMethods.AddAsync(method);
            await _context.RoomStatuses.AddAsync(status);
            await _context.RevenueSources.AddAsync(revenue);
            await _context.SaveChangesAsync();

            var model = new ReceptionistBookingCreateViewModel
            {
                Email = "clean@hotel.com",
                FirstName = "Clean",
                LastName = "Now",
                RoomId = room.Id,
                CheckInDate = today,
                CheckOutDate = today.AddDays(2),
                NumberOfGuests = 1,
                PaymentMethodId = method.Id
            };

            await _service.CreateBookingAsync(model);

            var updatedStatus = await _context.RoomStatuses.FirstOrDefaultAsync(s => s.RoomId == room.Id);
            Assert.That(updatedStatus!.Status, Is.EqualTo(RoomStatus.Occupied));
        }


        [Test]
        public void CreateBookingAsync_Throws_WhenRevenueSourceMissing()
        {
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "ErrorRoom",
                Description = "Will fail",
                PricePerNight = 100,
                MaxCapacity = 2,
                ImageUrl = "/fail.jpg",
                RoomType = RoomType.Standard
            };

            var method = new PaymentMethod
            {
                Id = Guid.NewGuid(),
                Name = "Bank Transfer"
            };

            var status = new StatusOfRoom
            {
                Id = Guid.NewGuid(),
                RoomId = room.Id,
                Status = RoomStatus.Available
            };

            _context.Rooms.Add(room);
            _context.PaymentMethods.Add(method);
            _context.RoomStatuses.Add(status);
            _context.SaveChanges();

            var model = new ReceptionistBookingCreateViewModel
            {
                Email = "fail@fail.com",
                FirstName = "Will",
                LastName = "Fail",
                RoomId = room.Id,
                CheckInDate = DateTime.UtcNow,
                CheckOutDate = DateTime.UtcNow.AddDays(1),
                NumberOfGuests = 1,
                PaymentMethodId = method.Id
            };

            Assert.ThrowsAsync<Exception>(async () => await _service.CreateBookingAsync(model));
        }


        [Test]
        public async Task GetRoomsOutForCleaningAsync_ReturnsCorrectRooms()
        {
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room A",
                Description = "Room that needs cleaning",
                ImageUrl = "/img.jpg",
                MaxCapacity = 2,
                RoomType = RoomType.Standard,
                PricePerNight = 120
            };

            var status = new StatusOfRoom
            {
                Id = Guid.NewGuid(),
                RoomId = room.Id,
                Status = RoomStatus.OutForCleaning
            };

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                RoomId = room.Id,
                CheckInDate = DateTime.UtcNow.AddDays(-3),
                CheckOutDate = DateTime.UtcNow.AddDays(-1),
                NumberOfGuests = 2,
                TotalAmount = 240,
                UserId = Guid.NewGuid().ToString(),
                PaymentMethodId = Guid.NewGuid(),
                BookingStatus = BookingStatus.Confirmed,
                PaymentStatus = PaymentStatus.Paid,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Rooms.AddAsync(room);
            await _context.RoomStatuses.AddAsync(status);
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            var result = await _service.GetRoomsOutForCleaningAsync();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].RoomName, Is.EqualTo("Room A"));
            Assert.That(result[0].LastCheckOutDate!.Value.Date, Is.EqualTo(booking.CheckOutDate.Date));
        }


        [Test]
        public async Task MarkRoomAsCleanedAsync_UpdatesRoomStatusToAvailable()
        {
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "CleanMe",
                Description = "To clean",
                ImageUrl = "/room.jpg",
                IsActive = false,
                MaxCapacity = 2,
                PricePerNight = 100,
                RoomType = RoomType.Standard
            };

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                RoomId = room.Id,
                CheckInDate = DateTime.UtcNow.AddDays(-2),
                CheckOutDate = DateTime.UtcNow.AddDays(-1),
                NumberOfGuests = 1,
                TotalAmount = 100,
                UserId = "user123",
                PaymentMethodId = Guid.NewGuid(),
                BookingStatus = BookingStatus.Confirmed,
                PaymentStatus = PaymentStatus.Paid,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var status = new StatusOfRoom
            {
                Id = Guid.NewGuid(),
                RoomId = room.Id,
                Status = RoomStatus.OutForCleaning
            };

            await _context.Rooms.AddAsync(room);
            await _context.Bookings.AddAsync(booking);
            await _context.RoomStatuses.AddAsync(status);
            await _context.SaveChangesAsync();

            var result = await _service.MarkRoomAsCleanedAsync(room.Id);

            var updatedStatus = await _context.RoomStatuses.FirstAsync(s => s.RoomId == room.Id);
            var updatedRoom = await _context.Rooms.FirstAsync(r => r.Id == room.Id);

            Assert.That(result, Is.True);
            Assert.That(updatedStatus.Status, Is.EqualTo(RoomStatus.Available));
            Assert.That(updatedRoom.IsActive, Is.True);
        }


        [Test]
        public async Task MarkRoomAsCleanedAsync_ReturnsFalse_IfCheckOutNotPassed()
        {
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "TooEarly",
                Description = "Future booking",
                ImageUrl = "/future.jpg",
                MaxCapacity = 2,
                PricePerNight = 150,
                RoomType = RoomType.Suite
            };

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                RoomId = room.Id,
                CheckInDate = DateTime.UtcNow,
                CheckOutDate = DateTime.UtcNow.AddDays(2), // in future
                NumberOfGuests = 1,
                TotalAmount = 300,
                UserId = "user456",
                PaymentMethodId = Guid.NewGuid(),
                BookingStatus = BookingStatus.Confirmed,
                PaymentStatus = PaymentStatus.Paid,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var status = new StatusOfRoom
            {
                Id = Guid.NewGuid(),
                RoomId = room.Id,
                Status = RoomStatus.OutForCleaning
            };

            await _context.Rooms.AddAsync(room);
            await _context.Bookings.AddAsync(booking);
            await _context.RoomStatuses.AddAsync(status);
            await _context.SaveChangesAsync();

            var result = await _service.MarkRoomAsCleanedAsync(room.Id);

            Assert.That(result, Is.False);
        }


        [Test]
        public async Task UpdateRoomsThatNeedCleaningAsync_ChangesStatusToOutForCleaning()
        {
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "NeedsCleaning",
                Description = "To mark",
                ImageUrl = "/img.jpg",
                MaxCapacity = 2,
                PricePerNight = 120,
                RoomType = RoomType.Standard
            };

            var status = new StatusOfRoom
            {
                Id = Guid.NewGuid(),
                RoomId = room.Id,
                Status = RoomStatus.Occupied
            };

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                RoomId = room.Id,
                CheckInDate = DateTime.UtcNow.AddDays(-3),
                CheckOutDate = DateTime.UtcNow.AddDays(-1),
                NumberOfGuests = 2,
                TotalAmount = 240,
                UserId = "user999",
                PaymentMethodId = Guid.NewGuid(),
                BookingStatus = BookingStatus.Confirmed,
                PaymentStatus = PaymentStatus.Paid,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Rooms.AddAsync(room);
            await _context.RoomStatuses.AddAsync(status);
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            await _service.UpdateRoomsThatNeedCleaningAsync();

            var updated = await _context.RoomStatuses.FirstOrDefaultAsync(rs => rs.RoomId == room.Id);
            Assert.That(updated!.Status, Is.EqualTo(RoomStatus.OutForCleaning));
        }

        
        public static class MockUserManager
        {
            public static UserManager<User> CreateMock(IUserStore<User> store)
            {
                var mgr = new Mock<UserManager<User>>(store, null, null, null, null, null, null, null, null);
                mgr.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);
                mgr.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
                return mgr.Object;
            }
        }

    }
}
