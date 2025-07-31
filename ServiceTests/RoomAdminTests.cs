using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.Services.Core;
using GrandHotelPetrichMVC.ViewModels.Admin.Room;
using Microsoft.EntityFrameworkCore;

namespace ServiceTests
{
    [TestFixture]
    public class RoomAdminTests
    {
        private ApplicationDbContext _context;
        private RoomAdminService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new RoomAdminService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAllRoomsAsync_ReturnsAllRooms()
        {
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 1",
                RoomType = RoomType.AccessibleRoom,
                Description = "Test Room",
                PricePerNight = 150,
                MaxCapacity = 2,
                ImageUrl = "img.jpg",
                IsActive = true
            };

            _context.Rooms.Add(room);
            _context.RoomStatuses.Add(new StatusOfRoom
            {
                RoomId = room.Id,
                Status = RoomStatus.Available,
                UpdatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            var result = await _service.GetAllRoomsAsync();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Room 1"));
            Assert.That(result[0].Status, Is.EqualTo(RoomStatus.Available));
        }

        [Test]
        public async Task CreateRoomAsync_AddsRoomWithStatusAndAmenities()
        {
            var amenity = new Amenity { Id = Guid.NewGuid(), Name = "WiFi", IsActive = true };
            await _context.Amenities.AddAsync(amenity);
            await _context.SaveChangesAsync();

            var model = new RoomCreateViewModel
            {
                Name = "Room A",
                RoomType = RoomType.Suite,
                PricePerNight = 100,
                MaxCapacity = 1,
                BedConfiguration = BedConfiguration.Single,
                Description = "Nice room",
                ImageUrl = "image.jpg",
                Badge = RoomBadge.New,
                SelectedAmenityIds = new List<Guid> { amenity.Id }
            };

            var result = await _service.CreateRoomAsync(model);

            Assert.That(result, Is.True);
            Assert.That(await _context.Rooms.CountAsync(), Is.EqualTo(1));
            Assert.That(await _context.RoomStatuses.CountAsync(), Is.EqualTo(1));
            Assert.That(await _context.RoomAmenities.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateRoomAsync_UpdatesRoomProperties()
        {
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Old Room",
                RoomType = RoomType.Deluxe,
                Description = "Test Desc",
                ImageUrl = "img.jpg",
                PricePerNight = 120,
                MaxCapacity = 2
            };
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var model = new RoomCreateViewModel
            {
                Id = room.Id,
                Name = "Updated Room",
                RoomType = RoomType.Suite,
                PricePerNight = 300,
                MaxCapacity = 4,
                BedConfiguration = BedConfiguration.Twin,
                Description = "Updated",
                ImageUrl = "new.jpg",
                Badge = RoomBadge.Popular
            };

            var result = await _service.UpdateRoomAsync(model);

            var updated = await _context.Rooms.FindAsync(room.Id);

            Assert.That(result, Is.True);
            Assert.That(updated!.Name, Is.EqualTo("Updated Room"));
            Assert.That(updated.RoomType, Is.EqualTo(RoomType.Suite));
        }

        [Test]
        public async Task ChangeRoomStatusAsync_UpdatesStatus()
        {
            var roomId = Guid.NewGuid();
            var room = new Room
            {
                Id = roomId,
                Name = "Test Room",
                RoomType = RoomType.Deluxe,
                PricePerNight = 100,
                MaxCapacity = 2,
                Description = "Sample description",
                ImageUrl = "/images/test.jpg",
                BedConfiguration = BedConfiguration.Single,
                Badge = RoomBadge.New,
                IsActive = true
            };

            var status = new StatusOfRoom
            {
                RoomId = roomId,
                Status = RoomStatus.Available,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Rooms.Add(room);
            _context.RoomStatuses.Add(status);
            await _context.SaveChangesAsync();

            var result = await _service.ChangeRoomStatusAsync(roomId, RoomStatus.Occupied);

            var updatedStatus = await _context.RoomStatuses.FirstAsync(s => s.RoomId == roomId);

            Assert.That(result, Is.True);
            Assert.That(updatedStatus.Status, Is.EqualTo(RoomStatus.Occupied));
        }



        [Test]
        public async Task ToggleRoomActiveAsync_ChangesActiveStatus()
        {
            var room = new Room 
            { 
                Id = Guid.NewGuid(), 
                Name = "Inactive Room",
                Description = "Test Desc",
                ImageUrl = "img.jpg",
                IsActive = false 
            };
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var result = await _service.ToggleRoomActiveAsync(room.Id, true);

            var updated = await _context.Rooms.FindAsync(room.Id);

            Assert.That(result, Is.True);
            Assert.That(updated!.IsActive, Is.True);
        }


        [Test]
        public async Task GetRoomByIdAsync_ReturnsMappedRoomViewModel()
        {
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Room 1",
                RoomType = RoomType.Deluxe,
                Description = "Test Desc",
                ImageUrl = "img.jpg",
                Badge = RoomBadge.New,
                PricePerNight = 100,
                MaxCapacity = 2,
                BedConfiguration = BedConfiguration.Single
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var model = await _service.GetRoomByIdAsync(room.Id);

            Assert.That(model, Is.Not.Null);
            Assert.That(model!.Name, Is.EqualTo("Room 1"));
            Assert.That(model.Badge, Is.EqualTo(RoomBadge.New));
        }


        [Test]
        public async Task GetAmenitiesSelectListAsync_ReturnsOnlyActiveAmenities()
        {
            _context.Amenities.AddRange(
                new Amenity { Id = Guid.NewGuid(), Name = "WiFi", IsActive = true },
                new Amenity { Id = Guid.NewGuid(), Name = "TV", IsActive = false }
            );
            await _context.SaveChangesAsync();

            var result = await _service.GetAmenitiesSelectListAsync();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Text, Is.EqualTo("WiFi"));
        }

    }
}
