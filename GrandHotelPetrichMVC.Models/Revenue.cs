using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Revenue;

namespace GrandHotelPetrichMVC.Data.Models
{
    public class Revenue
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? PaymentMethodId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        public Guid RevenueSourceId { get; set; }

        [MaxLength(DescriptionMaxLength)]
        public string? Description { get; set; }

        public Guid? BookingId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(BookingId))]
        public virtual Booking Booking { get; set; } = null!;

        [ForeignKey(nameof(PaymentMethodId))]
        public virtual PaymentMethod PaymentMethod { get; set; } = null!;

        [ForeignKey(nameof(RevenueSourceId))]
        public virtual RevenueSource Source { get; set; } = null!;
    }
}
