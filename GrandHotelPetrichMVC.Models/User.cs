using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.User;

namespace GrandHotelPetrichMVC.Data.Models
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(FirstNameMaxLength)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(LastNameMaxLength)]
        public string LastName { get; set; } = null!;

        [MaxLength(AddressMaxLength)]
        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual Staff? StaffProfile { get; set; } // One-to-one with Staff
    }
}
