using System.ComponentModel.DataAnnotations;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Booking;

namespace GrandHotelPetrichMVC.Data.Models
{
    public class PaymentMethod
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(PaymentMethodMaxLength)]
        public string Name { get; set; } = null!; // e.g. "Cash", "Credit Card", "PayPal"

        public bool IsActive { get; set; } = true;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}
