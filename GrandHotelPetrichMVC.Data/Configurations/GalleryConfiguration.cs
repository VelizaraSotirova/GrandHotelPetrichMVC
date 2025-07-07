using GrandHotelPetrichMVC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrandHotelPetrichMVC.Data.Configurations
{
    public class GalleryConfiguration : IEntityTypeConfiguration<Gallery>
    {
        public void Configure(EntityTypeBuilder<Gallery> builder)
        {
            builder
                .Property(g => g.DisplayOrder)
                .HasDefaultValue(0);

            builder
                .Property(g => g.IsActive)
                .HasDefaultValue(true);

            builder
                .Property(g => g.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder
                .Property(g => g.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
