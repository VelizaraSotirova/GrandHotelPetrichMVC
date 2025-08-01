using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core;
using GrandHotelPetrichMVC.ViewModels.Admin.Staff;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ServiceTests
{
    [TestFixture]
    public class StaffTests
    {
        private ApplicationDbContext _context = null!;
        private Mock<UserManager<User>> _userManagerMock = null!;
        private StaffService _service = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            _service = new StaffService(_context, _userManagerMock.Object);
        }

        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public async Task ChangeStaffStatusAsync_UpdatesStatus()
        {
            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                UserId = "user123",
                Status = StaffStatus.Active
            };
            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();

            var result = await _service.ChangeStaffStatusAsync(staff.Id, "OnLeave");

            Assert.IsTrue(result);
            var updated = await _context.Staff.FindAsync(staff.Id);
            Assert.That(updated!.Status, Is.EqualTo(StaffStatus.OnLeave));
        }

        [Test]
        public async Task ChangeStaffStatusAsync_InvalidId_ReturnsFalse()
        {
            var result = await _service.ChangeStaffStatusAsync(Guid.NewGuid(), "Terminated");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task ChangeStaffStatusAsync_InvalidStatus_ReturnsFalse()
        {
            var staff = new Staff { Id = Guid.NewGuid(), UserId = "user123", Status = StaffStatus.Active };
            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();

            var result = await _service.ChangeStaffStatusAsync(staff.Id, "UnknownStatus");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetEligibleUsersAsync_ReturnsOnlyNonStaffUsers()
        {
            var user1 = new User { Id = "1", FirstName = "Ivan", LastName = "Ivanov", Email = "a@test.com" };
            var user2 = new User { Id = "2", FirstName = "Peter", LastName = "Petrov", Email = "b@test.com" };

            _context.Users.AddRange(user1, user2);
            _context.Staff.Add(new Staff { Id = Guid.NewGuid(), UserId = "1" });
            await _context.SaveChangesAsync();

            var result = await _service.GetEligibleUsersAsync();
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo("2"));
        }

        [Test]
        public async Task CreateStaffAsync_CreatesStaffSuccessfully()
        {
            var user = new User
            { 
                Id = "user123",
                FirstName = "John",
                LastName = "Doe",
                Email = "staff@example.com" 
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _userManagerMock.Setup(m => m.FindByEmailAsync("staff@example.com"))
                .ReturnsAsync(user);
            _userManagerMock.Setup(m => m.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Customer"))
                .ReturnsAsync(true);
            _userManagerMock.Setup(m => m.RemoveFromRoleAsync(user, "Customer"))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Receptionist"))
                .ReturnsAsync(false);
            _userManagerMock.Setup(m => m.AddToRoleAsync(user, "Receptionist"))
                .ReturnsAsync(IdentityResult.Success);

            var model = new CreateStaffViewModel
            {
                UserEmail = "staff@example.com",
                PhoneNumber = "123456",
                Role = StaffRole.Receptionist,
                Salary = 1000,
                Shift = StaffShifts.Morning
            };

            var result = await _service.CreateStaffAsync(model);

            Assert.IsTrue(result);
            Assert.That(_context.Staff.Count(), Is.EqualTo(1));
            var staff = await _context.Staff.FirstOrDefaultAsync();
            Assert.That(staff!.Role, Is.EqualTo(StaffRole.Receptionist));
            Assert.That(staff.UserId, Is.EqualTo("user123"));
        }

        [Test]
        public async Task CreateStaffAsync_UserNotFound_ReturnsFalse()
        {
            var model = new CreateStaffViewModel
            {
                UserEmail = "notfound@example.com",
                PhoneNumber = "123456",
                Role = StaffRole.Housekeeping,
                Salary = 2000,
                Shift = StaffShifts.Evening
            };

            var result = await _service.CreateStaffAsync(model);

            Assert.IsFalse(result);
            Assert.That(_context.Staff.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task CreateStaffAsync_UserAlreadyInCorrectRole_SkipsAddingRole()
        {
            var user = new User
            {
                Id = "staff123",
                FirstName = "Emily",
                LastName = "White",
                Email = "emily@hotel.com"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _userManagerMock.Setup(m => m.FindByEmailAsync(user.Email))
                .ReturnsAsync(user);
            _userManagerMock.Setup(m => m.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Customer"))
                .ReturnsAsync(false);
            _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Cleaner"))
                .ReturnsAsync(true); // already has role
                                     // No AddToRoleAsync call expected

            var model = new CreateStaffViewModel
            {
                UserEmail = user.Email,
                PhoneNumber = "888888",
                Role = StaffRole.Housekeeping,
                Salary = 950,
                Shift = StaffShifts.Night
            };

            var result = await _service.CreateStaffAsync(model);

            Assert.IsTrue(result);
            Assert.That(_context.Staff.Count(), Is.EqualTo(1));
        }


        [Test]
        public async Task GetEligibleUsersDropdownAsync_ReturnsFormattedSelectListItems()
        {
            var user = new User
            {
                Id = "1",
                FirstName = "Anna",
                LastName = "Smith",
                Email = "anna@hotel.com"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _service.GetEligibleUsersDropdownAsync();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Text, Is.EqualTo("Anna Smith (anna@hotel.com)"));
            Assert.That(result[0].Value, Is.EqualTo("1"));
        }

    }
}


