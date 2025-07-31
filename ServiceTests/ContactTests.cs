using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services.Core;
using GrandHotelPetrichMVC.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ServiceTests
{
    [TestFixture]
    public class ContactTests
    {
        private ApplicationDbContext _context;
        private ContactService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new ContactService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task SubmitContactFormAsync_AddsMessageSuccessfully()
        {
            // Arrange
            var model = new ContactMessageFormViewModel
            {
                Name = "Test User",
                Email = "test@example.com",
                Phone = "0888123456",
                Subject = "Question",
                Message = "Hello, I need help."
            };

            // Act
            var result = await _service.SubmitContactFormAsync(model);

            // Assert
            Assert.IsTrue(result);
            Assert.That(await _context.ContactMessages.CountAsync(), Is.EqualTo(1));

            var savedMessage = await _context.ContactMessages.FirstAsync();
            Assert.That(savedMessage.Name, Is.EqualTo("Test User"));
            Assert.That(savedMessage.Email, Is.EqualTo("test@example.com"));
            Assert.That(savedMessage.PhoneNumber, Is.EqualTo("0888123456"));
        }

        [Test]
        public async Task GetPrefilledContactFormAsync_ReturnsData_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@example.com",
                PhoneNumber = "0888999888"
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var model = await _service.GetPrefilledContactFormAsync(user.Email);

            // Assert
            Assert.That(model.Name, Is.EqualTo("Jane Doe"));
            Assert.That(model.Email, Is.EqualTo("jane@example.com"));
            Assert.That(model.Phone, Is.EqualTo("0888999888"));
        }

        [Test]
        public async Task GetPrefilledContactFormAsync_ReturnsEmpty_WhenEmailIsNullOrNotFound()
        {
            // Act
            var nullEmailModel = await _service.GetPrefilledContactFormAsync(null);
            var unknownEmailModel = await _service.GetPrefilledContactFormAsync("notfound@example.com");

            // Assert
            Assert.IsNull(nullEmailModel.Name);
            Assert.IsNull(nullEmailModel.Email);
            Assert.IsNull(unknownEmailModel.Name);
        }
    }
}
