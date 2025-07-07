using GrandHotelPetrichMVC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrandHotelPetrichMVC.Data.Configurations
{
    public class StatusOfRoomConfiguration : IEntityTypeConfiguration<StatusOfRoom>
    {
        public void Configure(EntityTypeBuilder<StatusOfRoom> builder)
        {
            builder
                .Property(rs => rs.Status)
                .HasConversion<string>();

            builder
                .Property(rs => rs.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder
                .HasOne(rs => rs.Room)
                .WithOne()
                .HasForeignKey<StatusOfRoom>(rs => rs.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder
                .HasOne(rs => rs.CurrentBooking)
                .WithMany()
                .HasForeignKey(rs => rs.CurrentBookingId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
