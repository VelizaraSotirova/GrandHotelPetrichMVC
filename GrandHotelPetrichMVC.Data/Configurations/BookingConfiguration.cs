using GrandHotelPetrichMVC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrandHotelPetrichMVC.Data.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder
                .Property(b => b.TotalAmount)
                .HasPrecision(10, 2);

            builder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder
                .Property(b => b.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(b => b.PaymentMethod)
                .WithMany(pm => pm.Bookings)
                .HasForeignKey(b => b.PaymentMethodId);
        }
    }
}
