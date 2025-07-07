using GrandHotelPetrichMVC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrandHotelPetrichMVC.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .HasIndex(u => u.Email)
                .IsUnique();

            builder
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder
                .Property(u => u.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(u => u.StaffProfile)
                   .WithOne(s => s.User)
                   .HasForeignKey<Staff>(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
