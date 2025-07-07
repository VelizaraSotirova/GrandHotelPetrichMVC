using GrandHotelPetrichMVC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrandHotelPetrichMVC.Data.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder
                .Property(r => r.IsApproved)
                .HasDefaultValue(false);

            builder
                .Property(r => r.IsFeatured)
                .HasDefaultValue(false);

            builder
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder
                .Property(r => r.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            builder
                .HasOne(r => r.Room)
                .WithMany(rm => rm.Reviews)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
