using GrandHotelPetrichMVC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrandHotelPetrichMVC.Data.Configurations
{
    public class GalleryCategoryConfiguration : IEntityTypeConfiguration<GalleryCategory>
    {
        public void Configure(EntityTypeBuilder<GalleryCategory> builder)
        {
            builder
                .Property(gc => gc.IsActive)
                .HasDefaultValue(true);

            builder
                .Property(gc => gc.DisplayOrder)
                .HasDefaultValue(0);
        }
    }
}
