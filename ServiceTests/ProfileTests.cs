using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services;
using GrandHotelPetrichMVC.ViewModels.Guests.Profile;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ServiceTests
{
    [TestFixture]
    public class ProfileTests
    {
        private Mock<UserManager<User>> _userManagerMock;
        private ProfileService _profileService;

        [SetUp]
        public void SetUp()
        {
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            _profileService = new ProfileService(_userManagerMock.Object);
        }

        [Test]
        public async Task GetProfileAsync_ReturnsProfile_WhenUserExists()
        {
            // Arrange
            var userId = "123";
            var user = new User
            {
                Id = userId,
                FirstName = "Alice",
                LastName = "Smith",
                PhoneNumber = "123456789",
                Address = "123 Main St",
                Email = "alice@example.com"
            };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _profileService.GetProfileAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result!.FirstName, Is.EqualTo("Alice"));
            Assert.That(result.Email, Is.EqualTo("alice@example.com"));
        }

        [Test]
        public async Task GetProfileAsync_ReturnsNull_WhenUserNotFound()
        {
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null!);

            var result = await _profileService.GetProfileAsync("invalid-id");

            Assert.IsNull(result);
        }

        [Test]
        public async Task UpdateProfileAsync_ReturnsFalse_WhenUserNotFound()
        {
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null!);

            var model = new ProfileViewModel
            {
                FirstName = "Test",
                LastName = "User",
                Address = "Test Address",
                PhoneNumber = "123",
            };

            var result = await _profileService.UpdateProfileAsync("missing-user", model);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task UpdateProfileAsync_UpdatesFields_AndReturnsTrue()
        {
            // Arrange
            var user = new User
            {
                Id = "321",
                FirstName = "Old",
                LastName = "Name",
                PhoneNumber = "000",
                Address = "Old Address"
            };

            _userManagerMock.Setup(um => um.FindByIdAsync(user.Id))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            var model = new ProfileViewModel
            {
                FirstName = "New",
                LastName = "Name",
                PhoneNumber = "111",
                Address = "New Address"
            };

            // Act
            var result = await _profileService.UpdateProfileAsync(user.Id, model);

            // Assert
            Assert.IsTrue(result);
            Assert.That(user.FirstName, Is.EqualTo("New"));
            Assert.That(user.PhoneNumber, Is.EqualTo("111"));
        }

        [Test]
        public async Task UpdateProfileAsync_ReturnsFalse_WhenUpdateFails()
        {
            var user = new User { Id = "x" };

            _userManagerMock.Setup(um => um.FindByIdAsync(user.Id))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

            var model = new ProfileViewModel();
            var result = await _profileService.UpdateProfileAsync(user.Id, model);

            Assert.IsFalse(result);
        }
    }
}
