using GrandHotelPetrichMVC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrandHotelPetrichMVC.Data.Configurations
{
    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder
                .Property(s => s.Salary)
                .HasPrecision(10, 2);

            builder
                .Property(s => s.Role)
                .HasConversion<string>();

            builder
                .Property(s => s.Status)
                .HasConversion<string>();

            builder
                .Property(s => s.Shift)
                .HasConversion<string>();

            builder
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder
                .Property(s => s.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
