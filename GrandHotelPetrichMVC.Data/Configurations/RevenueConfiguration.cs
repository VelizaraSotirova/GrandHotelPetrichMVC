using GrandHotelPetrichMVC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrandHotelPetrichMVC.Data.Configurations
{
    public class RevenueConfiguration : IEntityTypeConfiguration<Revenue>
    {
        public void Configure(EntityTypeBuilder<Revenue> builder)
        {
            builder
                .Property(r => r.Amount)
                .HasPrecision(10, 2);

            builder
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder
                .HasOne(r => r.Booking)
                .WithMany(b => b.Revenues)
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder
                .HasOne(r => r.Source)
                .WithMany(rs => rs.Revenues)
                .HasForeignKey(r => r.RevenueSourceId);
        }
    }
}
