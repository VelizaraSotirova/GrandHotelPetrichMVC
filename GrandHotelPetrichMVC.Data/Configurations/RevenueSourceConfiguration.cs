using GrandHotelPetrichMVC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrandHotelPetrichMVC.Data.Configurations
{
    public class RevenueSourceConfiguration : IEntityTypeConfiguration<RevenueSource>
    {
        public void Configure(EntityTypeBuilder<RevenueSource> builder)
        {
            builder
                .Property(rs => rs.IsActive)
                .HasDefaultValue(true);
        }
    }
}
