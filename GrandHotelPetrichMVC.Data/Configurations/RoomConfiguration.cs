using GrandHotelPetrichMVC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrandHotelPetrichMVC.Data.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.Property(r => r.PricePerNight)
                   .HasPrecision(10, 2);

            builder.Property(r => r.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(r => r.UpdatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(r => r.RoomType)
                   .HasConversion<string>();

            builder.Property(r => r.Badge)
                   .HasConversion<string>();

            builder.Property(r => r.BedConfiguration)
                    .HasConversion<string>();
        }
    }
}
