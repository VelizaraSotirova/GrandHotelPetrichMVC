using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services.Core;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ServiceTests
{
    [TestFixture]
    public class GalleryTests
    {
        private ApplicationDbContext _context;
        private Mock<IWebHostEnvironment> _mockEnv;
        private Mock<IFileService> _mockFileService;
        private GalleryService _galleryService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(e => e.WebRootPath).Returns("wwwroot");

            _mockFileService = new Mock<IFileService>();
            _galleryService = new GalleryService(_context, _mockEnv.Object, _mockFileService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAllImagesAsync_ReturnsExpectedImages()
        {
            var category = new GalleryCategory
            {
                Id = Guid.NewGuid(),
                Name = "Nature"
            };

            var image = new Gallery
            {
                Id = Guid.NewGuid(),
                ImageUrl = "/images/gallery/img1.jpg",
                Description = "Beautiful view",
                Title = "Nature Image",
                Category = category,
                CategoryId = category.Id,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.GalleryCategories.AddAsync(category);
            await _context.Galleries.AddAsync(image);
            await _context.SaveChangesAsync();

            var result = await _galleryService.GetAllImagesAsync();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().ImageUrl, Is.EqualTo(image.ImageUrl));
        }

        [Test]
        public async Task DeleteImageAsync_RemovesImageAndDeletesFile()
        {
            var image = new Gallery
            {
                Id = Guid.NewGuid(),
                ImageUrl = "/images/gallery/delete.jpg",
                Title = "Delete Test",
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Galleries.AddAsync(image);
            await _context.SaveChangesAsync();

            _mockFileService.Setup(f => f.FileExists(It.IsAny<string>())).Returns(true);

            var result = await _galleryService.DeleteImageAsync(image.Id);

            Assert.IsTrue(result);
            _mockFileService.Verify(f => f.DeleteFile(It.Is<string>(p => p.EndsWith("delete.jpg"))), Times.Once);
        }

        [Test]
        public async Task DeleteImageAsync_ReturnsFalse_IfImageNotFound()
        {
            var result = await _galleryService.DeleteImageAsync(Guid.NewGuid());
            Assert.IsFalse(result);
        }
    }
}
