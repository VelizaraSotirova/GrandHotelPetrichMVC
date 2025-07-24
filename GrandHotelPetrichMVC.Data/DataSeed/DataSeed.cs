using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.GCommon.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GrandHotelPetrichMVC.Data.DataSeed
{
    public class DataSeed
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Apply migrations
            context.Database.Migrate();

            // Seed only if no users or rooms exist
            if (context.Users.Any() || context.Rooms.Any())
                return;

            var users = SeedUsers(context);
            SeedReceptionist(context);

            var rooms = SeedRooms(context);
            SeedStatuses(context, rooms);

            var categories = SeedGalleryCategories(context);
            SeedGallery(context, categories);

            var creditCardId = SeedPaymentMethods(context);
            var bookings = SeedBookings(context, users, rooms, creditCardId);

            var sourceId = SeedRevenueSources(context);
            SeedRevenues(context, bookings, sourceId);

            SeedReviews(context, users, rooms);
            SeedMessages(context);

            context.SaveChanges();
        }

        private static List<User> SeedUsers(ApplicationDbContext context)
        {
            var hasher = new PasswordHasher<User>();
            var admin = new User
            {
                UserName = "admin@hotel.com",
                NormalizedUserName = "ADMIN@HOTEL.COM",
                Email = "admin@hotel.com",
                NormalizedEmail = "ADMIN@HOTEL.COM",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                PhoneNumber = "1111111111",
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");

            var customer = new User
            {
                UserName = "john@customer.com",
                NormalizedUserName = "JOHN@CUSTOMER.COM",
                Email = "john@customer.com",
                NormalizedEmail = "JOHN@CUSTOMER.COM",
                FirstName = "John",
                LastName = "Customer",
                EmailConfirmed = true,
                PhoneNumber = "2222222222",
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            customer.PasswordHash = hasher.HashPassword(customer, "Customer@123");

            var users = new List<User> { admin, customer };
            context.Users.AddRange(users);

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                User = admin,
                Role = StaffRole.Manager,
                Shift = StaffShifts.Morning,
                Status = StaffStatus.Active,
                Salary = 3000,
                HireDate = DateTime.UtcNow
            };

            context.Staff.Add(staff);

            return users;
        }

        private static void SeedReceptionist(ApplicationDbContext context)
        {
            var hasher = new PasswordHasher<User>();
            var email = "receptionist@hotel.com";

            var existingUser = context.Users.FirstOrDefault(u => u.Email == email);
            if (existingUser == null)
            {
                var receptionist = new User
                {
                    UserName = email,
                    NormalizedUserName = email.ToUpper(),
                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                    FirstName = "Reception",
                    LastName = "Staff",
                    EmailConfirmed = true,
                    PhoneNumber = "3333333333",
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                receptionist.PasswordHash = hasher.HashPassword(receptionist, "Reception@123");

                context.Users.Add(receptionist);

                // Add staff record
                var staff = new Staff
                {
                    Id = Guid.NewGuid(),
                    UserId = receptionist.Id,
                    Role = StaffRole.Receptionist,
                    Shift = StaffShifts.Evening,
                    Status = StaffStatus.Active,
                    Salary = 1500,
                    HireDate = DateTime.UtcNow
                };

                context.Staff.Add(staff);
            }
        }


        private static List<Room> SeedRooms(ApplicationDbContext context)
        {
            // Step 1: Create and seed amenities
            var wifi = new Amenity { Id = Guid.NewGuid(), Name = "Wi-Fi" };
            var tv = new Amenity { Id = Guid.NewGuid(), Name = "Television" };
            var minibar = new Amenity { Id = Guid.NewGuid(), Name = "Mini Bar" };
            var ac = new Amenity { Id = Guid.NewGuid(), Name = "Air Conditioning" };
            var coffee = new Amenity { Id = Guid.NewGuid(), Name = "Coffee Maker" };

            context.Amenities.AddRange(wifi, tv, minibar, ac, coffee);

            // Step 2: Create rooms
            var room1 = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Deluxe Room",
                RoomType = RoomType.Deluxe,
                PricePerNight = 200,
                MaxCapacity = 2,
                BedConfiguration = BedConfiguration.KingPlusSofa,
                Description = "Spacious deluxe room",
                Badge = RoomBadge.Popular,
                ImageUrl = "/images/rooms/deluxe.jpg"
            };

            var room2 = new Room
            {
                Id = Guid.NewGuid(),
                Name = "Standard Room",
                RoomType = RoomType.Standard,
                PricePerNight = 100,
                MaxCapacity = 2,
                BedConfiguration = BedConfiguration.Twin,
                Description = "Comfortable standard room",
                ImageUrl = "/images/rooms/standard-double.jpg"
            };

            context.Rooms.AddRange(room1, room2);

            // Step 3: Link rooms to amenities
            var roomAmenities = new List<RoomAmenity>
            {
                new RoomAmenity { RoomId = room1.Id, AmenityId = wifi.Id },
                new RoomAmenity { RoomId = room1.Id, AmenityId = ac.Id },
                new RoomAmenity { RoomId = room1.Id, AmenityId = minibar.Id },

                new RoomAmenity { RoomId = room2.Id, AmenityId = wifi.Id },
                new RoomAmenity { RoomId = room2.Id, AmenityId = tv.Id },
                new RoomAmenity { RoomId = room2.Id, AmenityId = coffee.Id }
            };

            context.RoomAmenities.AddRange(roomAmenities);

            return new List<Room> { room1, room2 };
        }


        private static void SeedStatuses(ApplicationDbContext context, List<Room> rooms)
        {
            var statuses = rooms.Select(room => new StatusOfRoom
            {
                Id = Guid.NewGuid(),
                RoomId = room.Id,
                Status = RoomStatus.Available,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            context.RoomStatuses.AddRange(statuses);
        }

        private static List<GalleryCategory> SeedGalleryCategories (ApplicationDbContext context)
        {
            var categories = new List<GalleryCategory>
            {
                new GalleryCategory { Id = Guid.NewGuid(), Name = "Lobby" },
                new GalleryCategory { Id = Guid.NewGuid(), Name = "Pool" },
                new GalleryCategory { Id = Guid.NewGuid(), Name = "Restaurant" },
                new GalleryCategory { Id = Guid.NewGuid(), Name = "Spa" },
                new GalleryCategory { Id = Guid.NewGuid(), Name = "Suite" }
            };
            context.GalleryCategories.AddRange(categories);
            return categories;
        }

        private static void SeedGallery(ApplicationDbContext context, List<GalleryCategory> categories)
        {
            var items = new List<Gallery>
            {
                new Gallery { Id = Guid.NewGuid(), Title = "Lobby", Description="Hotel Lobby", CategoryId = categories.First().Id , ImageUrl = "/images/gallery/lobby.jpg", DisplayOrder = 1 },
                new Gallery { Id = Guid.NewGuid(), Title = "Pool", Description="Hotel Pool", CategoryId = categories.First().Id, ImageUrl = "/images/gallery/pool.jpg", DisplayOrder = 2 },
                new Gallery { Id = Guid.NewGuid(), Title = "Restaurant", Description="Hotel Restaurant", CategoryId = categories.First().Id , ImageUrl = "/images/gallery/restaurant.jpg", DisplayOrder = 3 },
                new Gallery { Id = Guid.NewGuid(), Title = "Spa", Description="Hotel Spa", CategoryId = categories.First().Id ,ImageUrl = "/images/gallery/spa.jpg", DisplayOrder = 4 },
                new Gallery { Id = Guid.NewGuid(), Title = "Suite", Description="Suite", CategoryId = categories.First().Id , ImageUrl = "/images/gallery/suite.jpg", DisplayOrder = 5 },
            };

            context.Galleries.AddRange(items);
        }


        private static Guid SeedPaymentMethods(ApplicationDbContext context)
        {
            var methodsToSeed = new List<PaymentMethod>
            {
                new PaymentMethod { Id = Guid.NewGuid(), Name = "Credit Card" },
                new PaymentMethod { Id = Guid.NewGuid(), Name = "PayPal" },
                new PaymentMethod { Id = Guid.NewGuid(), Name = "Cash" }
            };

            context.PaymentMethods.AddRange(methodsToSeed);

            return methodsToSeed.First().Id; // Return the first method's ID for use in bookings
        }


        private static List<Booking> SeedBookings(ApplicationDbContext context, List<User> users, List<Room> rooms, Guid creditCardId)
        {
            var bookings = new List<Booking>
            {
                new Booking
                {
                    Id = Guid.NewGuid(),
                    UserId = users.First(u => u.Email == "john@customer.com").Id,
                    RoomId = rooms.First().Id,
                    CheckInDate = DateTime.UtcNow.Date,
                    CheckOutDate = DateTime.UtcNow.AddDays(3).Date,
                    NumberOfGuests = 2,
                    TotalAmount = 600m,
                    BookingStatus = BookingStatus.Confirmed,
                    PaymentStatus = PaymentStatus.Paid,
                    PaymentMethodId = creditCardId,
                    IsCheckedIn = true,
                    ActualCheckIn = DateTime.UtcNow.Date,
                    SpecialRequests = "Late check-in"
                }
            };

            context.Bookings.AddRange(bookings);
            return bookings;
        }

        private static Guid SeedRevenueSources(ApplicationDbContext context)
        {
            var sources = new List<RevenueSource>
                {
                    new RevenueSource { Id = Guid.NewGuid(), Name = "Room" },
                    new RevenueSource { Id = Guid.NewGuid(), Name = "Restaurant" },
                    new RevenueSource { Id = Guid.NewGuid(), Name = "Spa" }
                };

            context.RevenueSources.AddRange(sources);

            return sources.First().Id; // Return the first source's ID for use in revenues
        }

        private static void SeedRevenues(ApplicationDbContext context, List<Booking> bookings, Guid sourceId)
        {
            var revenues = bookings.Select(b => new Revenue
            {
                Id = Guid.NewGuid(),
                BookingId = b.Id,
                Amount = b.TotalAmount,
                Date = b.CheckInDate,
                Description = $"Room booking: {b.NumberOfGuests} guests, {b.CheckInDate:MMM dd} - {b.CheckOutDate:MMM dd}",
                RevenueSourceId = sourceId,
                PaymentMethodId = b.PaymentMethodId
            }).ToList();

            context.Revenues.AddRange(revenues);
        }

        private static void SeedReviews(ApplicationDbContext context, List<User> users, List<Room> rooms)
        {
            var reviews = new List<Review>
            {
                new Review
                {
                    Id = Guid.NewGuid(),
                    UserId = users[1].Id,
                    RoomId = rooms[0].Id,
                    Rating = 5,
                    Title = "Excellent Stay",
                    Comment = "Very clean and comfortable",
                    IsApproved = true,
                    IsFeatured = true
                }
            };

            context.Reviews.AddRange(reviews);
        }

        private static void SeedMessages(ApplicationDbContext context)
        {
            var messages = new List<ContactMessage>
            {
                new ContactMessage
                {
                    Id = Guid.NewGuid(),
                    Name = "Alice",
                    Email = "alice@example.com",
                    Subject = "Availability",
                    Message = "Do you have rooms for the weekend?",
                    Status = ContactMessageStatus.New
                }
            };

            context.ContactMessages.AddRange(messages);
        }
    }
}
