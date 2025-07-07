using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GrandHotelPetrichMVC.GCommon.Enums;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.StatusOfRoom;

namespace GrandHotelPetrichMVC.Data.Models
{
    public class StatusOfRoom
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid RoomId { get; set; }

        [Required]
        public RoomStatus Status { get; set; } = RoomStatus.Available;

        [Required]
        public DateTime LastCleaned { get; set; }

        public DateTime? LastInspected { get; set; }

        public Guid? CurrentBookingId { get; set; }

        [MaxLength(MaxNotesLength)]
        public string? Notes { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(RoomId))]
        public virtual Room Room { get; set; } = null!;

        [ForeignKey(nameof(CurrentBookingId))]
        public virtual Booking? CurrentBooking { get; set; }
    }
}
