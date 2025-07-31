using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services.Core;
using Microsoft.EntityFrameworkCore;

namespace ServiceTests
{
    [TestFixture]
    public class PaymentMethodTests
    {
        private ApplicationDbContext _dbContext;
        private PaymentMethodService _service;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _service = new PaymentMethodService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task GetAllMethodsAsync_ReturnsSortedList()
        {
            // Arrange
            var methods = new List<PaymentMethod>
            {
                new PaymentMethod { Id = Guid.NewGuid(), Name = "Card", IsActive = true },
                new PaymentMethod { Id = Guid.NewGuid(), Name = "Cash", IsActive = false },
                new PaymentMethod { Id = Guid.NewGuid(), Name = "PayPal", IsActive = true }
            };

            await _dbContext.PaymentMethods.AddRangeAsync(methods);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetAllMethodsAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].IsActive, Is.True); // active first
            Assert.That(result[0].Name, Is.EqualTo("Card")); // alphabetically
            Assert.That(result[1].Name, Is.EqualTo("PayPal"));
            Assert.That(result[2].IsActive, Is.False);
        }

        [Test]
        public async Task AddMethodAsync_ReturnsFalse_WhenNameIsNull()
        {
            var result = await _service.AddMethodAsync(null!);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddMethodAsync_ReturnsFalse_WhenNameIsWhitespace()
        {
            var result = await _service.AddMethodAsync("   ");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddMethodAsync_ReturnsFalse_WhenNameExists()
        {
            await _service.AddMethodAsync("Card");
            var result = await _service.AddMethodAsync("Card");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddMethodAsync_AddsValidMethod()
        {
            var result = await _service.AddMethodAsync("Crypto");
            Assert.IsTrue(result);

            var count = await _dbContext.PaymentMethods.CountAsync();
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task DisableMethodAsync_ReturnsFalse_IfNotFound()
        {
            var result = await _service.DisableMethodAsync(Guid.NewGuid());
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DisableMethodAsync_SetsIsActiveFalse()
        {
            var method = new PaymentMethod
            {
                Id = Guid.NewGuid(),
                Name = "Bank Transfer",
                IsActive = true
            };
            _dbContext.PaymentMethods.Add(method);
            await _dbContext.SaveChangesAsync();

            var result = await _service.DisableMethodAsync(method.Id);

            Assert.IsTrue(result);
            var updated = await _dbContext.PaymentMethods.FindAsync(method.Id);
            Assert.IsFalse(updated!.IsActive);
        }

        [Test]
        public async Task ReactivateMethodAsync_ReturnsFalse_IfNotFound()
        {
            var result = await _service.ReactivateMethodAsync(Guid.NewGuid());
            Assert.IsFalse(result);
        }

        [Test]
        public async Task ReactivateMethodAsync_SetsIsActiveTrue()
        {
            var method = new PaymentMethod
            {
                Id = Guid.NewGuid(),
                Name = "Bank Transfer",
                IsActive = false
            };
            _dbContext.PaymentMethods.Add(method);
            await _dbContext.SaveChangesAsync();

            var result = await _service.ReactivateMethodAsync(method.Id);

            Assert.IsTrue(result);
            var updated = await _dbContext.PaymentMethods.FindAsync(method.Id);
            Assert.IsTrue(updated!.IsActive);
        }
    }
}
