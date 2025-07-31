using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core;
using Microsoft.EntityFrameworkCore;

namespace ServiceTests
{
    [TestFixture]
    public class AdminTests
    {
        private DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private ApplicationDbContext _dbContext;
        private AdminService _adminService;
        private Guid _reviewId;

        [SetUp]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(_dbContextOptions);
            _adminService = new AdminService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public void AlwaysPasses()
        {
            Assert.Pass();
        }

        [Test]
        public async Task GetDashboardStatsAsync_ReturnsCorrectStatistics()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            // Create test data
            await SeedTestDataAsync(now, startOfMonth);

            // Act
            var result = await _adminService.GetDashboardStatsAsync();

            // Assert
            Assert.That(result.AvailableRooms, Is.EqualTo(2));
            Assert.That(result.OccupiedRooms, Is.EqualTo(1));
            Assert.That(result.TotalRooms, Is.EqualTo(3));
            Assert.That(result.StaffCount, Is.EqualTo(2));
            Assert.That(result.MonthlyRevenue, Is.EqualTo(2100)); // Only current month revenues
            Assert.That(result.PaidSalaries, Is.EqualTo(4500)); // Active + OnLeave staff
        }

        [Test]
        public async Task GetAllContactMessagesAsync_ReturnsMessages()
        {
            await SeedContactAndReviewTestData();

            var result = await _adminService.GetAllContactMessagesAsync();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Test User"));
            Assert.That(result.First().Subject, Is.EqualTo("Test Subject"));
        }

        [Test]
        public async Task GetAllReviewsAsync_ReturnsReviewsWithCorrectFields()
        {
            await SeedContactAndReviewTestData();

            var result = await _adminService.GetAllReviewsAsync();

            Assert.That(result.Count, Is.EqualTo(1));
            var review = result.First();
            Assert.That(review.UserFullName, Is.EqualTo("Alice Smith"));
            Assert.That(review.RoomName, Is.EqualTo("Ocean View"));
            Assert.That(review.IsApproved, Is.False);
        }

        [Test]
        public async Task ApproveReviewAsync_SetsApprovedTrue()
        {
            await SeedContactAndReviewTestData();

            var result = await _adminService.ApproveReviewAsync(_reviewId);

            Assert.IsTrue(result);

            var updatedReview = await _dbContext.Reviews
                .FirstOrDefaultAsync(r => r.Id == _reviewId);
            Assert.IsTrue(updatedReview!.IsApproved);
        }

        [Test]
        public async Task ApproveReviewAsync_ReturnsFalseIfNotFound()
        {
            var result = await _adminService.ApproveReviewAsync(Guid.NewGuid());

            Assert.IsFalse(result);
        }

        private async Task SeedContactAndReviewTestData()
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice@example.com"
            };

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Ocean View",
                PricePerNight = 200,
                MaxCapacity = 2,
                Description = "Bright room with a sea view",
                ImageUrl = "/images/test.jpg",
                IsActive = true
            };

            var review = new Review
            {
                Id = Guid.NewGuid(),
                Title = "Great Stay!",
                Comment = "Clean and comfy",
                IsApproved = false,
                CreatedAt = DateTime.UtcNow,
                Rating = 5,
                User = user,
                Room = room
            };

            _reviewId = review.Id;

            var message = new ContactMessage
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@guest.com",
                Subject = "Test Subject",
                Message = "Testing message content.",
                CreatedAt = DateTime.UtcNow,
                Status = ContactMessageStatus.New
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.Rooms.AddAsync(room);
            await _dbContext.Reviews.AddAsync(review);
            await _dbContext.ContactMessages.AddAsync(message);
            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedTestDataAsync(DateTime now, DateTime startOfMonth)
        {
            // Create payment method
            var paymentMethod = new PaymentMethod
            {
                Id = Guid.NewGuid(),
                Name = "Credit Card",
                IsActive = true
            };

            // Create revenue source
            var revenueSource = new RevenueSource
            {
                Id = Guid.NewGuid(),
                Name = "Room Bookings",
                IsActive = true
            };

            // Create rooms
            var rooms = new List<Room>
            {
                new Room
                {
                    Id = Guid.NewGuid(),
                    Name = "Deluxe Suite",
                    RoomType = RoomType.Suite,
                    PricePerNight = 250.00m,
                    MaxCapacity = 4,
                    BedConfiguration = BedConfiguration.Queen,
                    Description = "Luxury suite with ocean view",
                    ImageUrl = "/images/suite.jpg",
                    IsActive = true
                },
                new Room
                {
                    Id = Guid.NewGuid(),
                    Name = "Standard Room",
                    RoomType = RoomType.Standard,
                    PricePerNight = 120.00m,
                    MaxCapacity = 2,
                    BedConfiguration = BedConfiguration.Single,
                    Description = "Comfortable standard room",
                    ImageUrl = "/images/standard.jpg",
                    IsActive = true
                },
                new Room
                {
                    Id = Guid.NewGuid(),
                    Name = "Business Room",
                    RoomType = RoomType.BusinessRoom,
                    PricePerNight = 180.00m,
                    MaxCapacity = 2,
                    BedConfiguration = BedConfiguration.King,
                    Description = "Business class accommodations",
                    ImageUrl = "/images/executive.jpg",
                    IsActive = true
                }
            };

            // Create room statuses
            var roomStatuses = new List<StatusOfRoom>
            {
                new StatusOfRoom
                {
                    Id = Guid.NewGuid(),
                    RoomId = rooms[0].Id,
                    Status = RoomStatus.Available,
                    LastCleaned = now.AddHours(-2)
                },
                new StatusOfRoom
                {
                    Id = Guid.NewGuid(),
                    RoomId = rooms[1].Id,
                    Status = RoomStatus.Available,
                    LastCleaned = now.AddHours(-4)
                },
                new StatusOfRoom
                {
                    Id = Guid.NewGuid(),
                    RoomId = rooms[2].Id,
                    Status = RoomStatus.Occupied,
                    LastCleaned = now.AddDays(-1),
                    CurrentBookingId = Guid.NewGuid()
                }
            };

            // Create users (staff)
            var staffUsers = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "staff1@hotel.com",
                    Email = "staff1@hotel.com",
                    FirstName = "John",
                    LastName = "Doe",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true
                },
                new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "staff2@hotel.com",
                    Email = "staff2@hotel.com",
                    FirstName = "Jane",
                    LastName = "Smith",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true
                },
                new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "staff3@hotel.com",
                    Email = "staff3@hotel.com",
                    FirstName = "Bob",
                    LastName = "Johnson",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = false
                }
            };

            // Create staff profiles
            var staffMembers = new List<Staff>
            {
                new Staff
                {
                    Id = Guid.NewGuid(),
                    UserId = staffUsers[0].Id,
                    Role = StaffRole.Receptionist,
                    Shift = StaffShifts.Day,
                    Status = StaffStatus.Active,
                    HireDate = now.AddMonths(-6),
                    Salary = 2500.00m
                },
                new Staff
                {
                    Id = Guid.NewGuid(),
                    UserId = staffUsers[1].Id,
                    Role = StaffRole.Housekeeping,
                    Shift = StaffShifts.Evening,
                    Status = StaffStatus.OnLeave,
                    HireDate = now.AddMonths(-12),
                    Salary = 2000.00m
                },
                new Staff
                {
                    Id = Guid.NewGuid(),
                    UserId = staffUsers[2].Id,
                    Role = StaffRole.Housekeeping,
                    Shift = StaffShifts.Night,
                    Status = StaffStatus.Terminated,
                    HireDate = now.AddMonths(-24),
                    Salary = 1800.00m
                }
            };

            // First create the revenue sources
            var revenueSources = new List<RevenueSource>
            {
                new RevenueSource
                {
                    Id = Guid.NewGuid(),
                    Name = "Room Bookings",
                    IsActive = true,
                    DisplayOrder = 1
                },
                new RevenueSource
                {
                    Id = Guid.NewGuid(),
                    Name = "Restaurant",
                    IsActive = true,
                    DisplayOrder = 2
                },
                new RevenueSource
                {
                    Id = Guid.NewGuid(),
                    Name = "Spa Services",
                    IsActive = true,
                    DisplayOrder = 3
                }
            };

            // Then create revenues with proper source references
            var revenues = new List<Revenue>
            {
                new Revenue
                {
                    Id = Guid.NewGuid(),
                    Date = startOfMonth.AddDays(1),
                    Amount = 1250.00m,
                    RevenueSourceId = revenueSources[0].Id, // Room Bookings
                    Description = "Daily room revenue",
                    PaymentMethodId = paymentMethod.Id
                },
                new Revenue
                {
                    Id = Guid.NewGuid(),
                    Date = startOfMonth.AddDays(5),
                    Amount = 850.00m,
                    RevenueSourceId = revenueSources[1].Id, // Restaurant
                    Description = "Weekend restaurant revenue",
                    PaymentMethodId = paymentMethod.Id
                },
                new Revenue
                {
                    Id = Guid.NewGuid(),
                    Date = startOfMonth.AddMonths(-1),
                    Amount = 1000.00m,
                    RevenueSourceId = revenueSources[2].Id, // Spa Services
                    Description = "Monthly spa revenue",
                    PaymentMethodId = paymentMethod.Id
                }
            };

            // Add all entities to context
            await _dbContext.PaymentMethods.AddAsync(paymentMethod);
            await _dbContext.RevenueSources.AddAsync(revenueSource);
            await _dbContext.Rooms.AddRangeAsync(rooms);
            await _dbContext.RoomStatuses.AddRangeAsync(roomStatuses);
            await _dbContext.Users.AddRangeAsync(staffUsers);
            await _dbContext.Staff.AddRangeAsync(staffMembers);
            await _dbContext.Revenues.AddRangeAsync(revenues);

            await _dbContext.SaveChangesAsync();
        }
    }
}
