using GrandHotelPetrichMVC.GCommon.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Booking;

namespace GrandHotelPetrichMVC.Data.Models
{
    public class Booking
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(MaxLengthUserId)]
        public string UserId { get; set; } = null!;

        public Guid? PaymentMethodId { get; set; }

        [Required]
        public Guid RoomId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        public int NumberOfGuests { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public BookingStatus BookingStatus { get; set; } = BookingStatus.Pending; 

        [MaxLength(SpecialRequestsMaxLength)]
        public string? SpecialRequests { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        // New properties for check-in/check-out tracking
        public DateTime? ActualCheckIn { get; set; }

        public DateTime? ActualCheckOut { get; set; }

        public bool IsCheckedIn { get; set; } = false;

        public bool IsCheckedOut { get; set; } = false;

        [MaxLength(CheckInNotesMaxLength)]
        public string? CheckInNotes { get; set; }

        [MaxLength(CheckOutNotesMaxLength)]
        public string? CheckOutNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(RoomId))]
        public virtual Room Room { get; set; } = null!;

        [ForeignKey(nameof(PaymentMethodId))]
        public virtual PaymentMethod PaymentMethod { get; set; } = null!;

        // Reverse navigation
        public virtual ICollection<Revenue> Revenues { get; set; } = new List<Revenue>();
    }
}
