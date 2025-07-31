using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services.Core;
using GrandHotelPetrichMVC.ViewModels.Guests.Review;
using Microsoft.EntityFrameworkCore;

namespace ServiceTests
{
    [TestFixture]
    public class ReviewTests
    {
        private ApplicationDbContext _context;
        private ReviewService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new ReviewService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task CanUserSubmitReviewAsync_ReturnsTrue_WhenUserHasPastBooking()
        {
            var userId = "user1";

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CheckInDate = DateTime.UtcNow.AddDays(-5),
                CheckOutDate = DateTime.UtcNow.AddDays(-1),
                RoomId = Guid.NewGuid()
            };

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            var result = await _service.CanUserSubmitReviewAsync(userId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CanUserSubmitReviewAsync_ReturnsFalse_WhenNoPastBooking()
        {
            var userId = "user2";

            var result = await _service.CanUserSubmitReviewAsync(userId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task SubmitReviewAsync_ReturnsFalse_WhenNoCompletedStay()
        {
            var model = new CreateReviewViewModel
            {
                RoomId = Guid.NewGuid(),
                Title = "Good stay",
                Comment = "Everything was fine.",
                Rating = 5
            };

            var result = await _service.SubmitReviewAsync(model, "nonexistent-user");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task SubmitReviewAsync_ReturnsTrue_AndAddsReview_WhenStayCompleted()
        {
            var userId = "user3";
            var roomId = Guid.NewGuid();

            await _context.Bookings.AddAsync(new Booking
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoomId = roomId,
                CheckInDate = DateTime.UtcNow.AddDays(-5),
                CheckOutDate = DateTime.UtcNow.AddDays(-1)
            });

            await _context.SaveChangesAsync();

            var model = new CreateReviewViewModel
            {
                RoomId = roomId,
                Title = "Great Service",
                Comment = "Loved the room and the staff.",
                Rating = 5
            };

            var result = await _service.SubmitReviewAsync(model, userId);

            Assert.That(result, Is.True);

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.UserId == userId);
            Assert.That(review, Is.Not.Null);
            Assert.That(review!.Title, Is.EqualTo("Great Service"));
            Assert.That(review.IsApproved, Is.False);
        }

        [Test]
        public async Task GetApprovedReviewsAsync_ReturnsOnlyApprovedReviews()
        {
            var user = new User
            {
                Id = "user4",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com"
            };

            await _context.Users.AddAsync(user);

            var approvedReview = new Review
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RoomId = Guid.NewGuid(),
                Title = "Loved It",
                Comment = "Amazing service!",
                Rating = 5,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow
            };

            var unapprovedReview = new Review
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RoomId = Guid.NewGuid(),
                Title = "Not shown",
                Comment = "Hidden from view",
                Rating = 3,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Reviews.AddRangeAsync(approvedReview, unapprovedReview);
            await _context.SaveChangesAsync();

            var result = await _service.GetApprovedReviewsAsync();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Loved It"));
            Assert.That(result[0].UserFullName, Is.EqualTo("Test User"));
        }

    }
}
