using GrandHotelPetrichMVC.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace GrandHotelPetrichMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<RoomAmenity> RoomAmenities { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Revenue> Revenues { get; set; }
        public DbSet<RevenueSource> RevenueSources { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<GalleryCategory> GalleryCategories { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<StatusOfRoom> RoomStatuses { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
