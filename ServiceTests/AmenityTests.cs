using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services.Core;
using GrandHotelPetrichMVC.ViewModels.Admin.Amenity;
using Microsoft.EntityFrameworkCore;

namespace ServiceTests
{
    [TestFixture]
    public class AmenityTests
    {
        private ApplicationDbContext _dbContext;
        private AmenityService _amenityService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _amenityService = new AmenityService(_dbContext);
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
        public async Task GetAllAsync_ReturnsAllAmenities()
        {
            // Arrange
            var amenity1 = new Amenity { Id = Guid.NewGuid(), Name = "Wi-Fi", IsActive = true };
            var amenity2 = new Amenity { Id = Guid.NewGuid(), Name = "Parking", IsActive = false };

            await _dbContext.Amenities.AddRangeAsync(amenity1, amenity2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _amenityService.GetAllAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(a => a.Name == "Wi-Fi" && a.IsActive), Is.True);
            Assert.That(result.Any(a => a.Name == "Parking" && !a.IsActive), Is.True);
        }

        [Test]
        public async Task AddAsync_AddsNewAmenity_WhenNameIsUnique()
        {
            // Arrange
            var model = new AddAmenityViewModel { Name = "Pool" };

            // Act
            var result = await _amenityService.AddAsync(model);

            // Assert
            Assert.IsTrue(result);
            Assert.That(await _dbContext.Amenities.CountAsync(), Is.EqualTo(1));
            Assert.That((await _dbContext.Amenities.FirstAsync()).Name, Is.EqualTo("Pool"));
        }

        [Test]
        public async Task AddAsync_ReturnsFalse_WhenNameAlreadyExists_CaseInsensitive()
        {
            // Arrange
            await _dbContext.Amenities.AddAsync(new Amenity { Id = Guid.NewGuid(), Name = "Spa", IsActive = true });
            await _dbContext.SaveChangesAsync();

            var model = new AddAmenityViewModel { Name = "spa" };

            // Act
            var result = await _amenityService.AddAsync(model);

            // Assert
            Assert.IsFalse(result);
            Assert.That(await _dbContext.Amenities.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task DeactivateAsync_SetsAmenityToInactive()
        {
            // Arrange
            var amenity = new Amenity { Id = Guid.NewGuid(), Name = "Gym", IsActive = true };
            await _dbContext.Amenities.AddAsync(amenity);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _amenityService.DeactivateAsync(amenity.Id);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse((await _dbContext.Amenities.FindAsync(amenity.Id))!.IsActive);
        }

        [Test]
        public async Task DeactivateAsync_ReturnsFalse_WhenAmenityNotFound()
        {
            // Act
            var result = await _amenityService.DeactivateAsync(Guid.NewGuid());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task ActivateAsync_SetsAmenityToActive()
        {
            // Arrange
            var amenity = new Amenity { Id = Guid.NewGuid(), Name = "Sauna", IsActive = false };
            await _dbContext.Amenities.AddAsync(amenity);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _amenityService.ActivateAsync(amenity.Id);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue((await _dbContext.Amenities.FindAsync(amenity.Id))!.IsActive);
        }

        [Test]
        public async Task ActivateAsync_ReturnsFalse_WhenAmenityNotFound()
        {
            // Act
            var result = await _amenityService.ActivateAsync(Guid.NewGuid());

            // Assert
            Assert.IsFalse(result);
        }
    }
}
