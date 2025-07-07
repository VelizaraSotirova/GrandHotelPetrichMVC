using GrandHotelPetrichMVC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrandHotelPetrichMVC.Data.Configurations
{
    public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
    {
        public void Configure(EntityTypeBuilder<ContactMessage> builder)
        {
            builder
                .Property(cm => cm.Status)
                .HasConversion<string>();

            builder
                .Property(cm => cm.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder
                .Property(cm => cm.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
