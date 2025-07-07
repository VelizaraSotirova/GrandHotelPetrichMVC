using GrandHotelPetrichMVC.GCommon.Enums;
using System.ComponentModel.DataAnnotations;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.ContactMessage;

namespace GrandHotelPetrichMVC.Data.Models
{
    public class ContactMessage
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(EmailMaxLength)]
        public string Email { get; set; } = null!;

        [Phone]
        [MaxLength(PhoneNumberMaxLength)]
        public string? PhoneNumber { get; set; }

        [Required]
        [MaxLength(SubjectMaxLength)]
        public string Subject { get; set; } = null!;

        [Required]
        [MaxLength(MessageMaxLength)]
        public string Message { get; set; } = null!;

        [Required]
        public ContactMessageStatus Status { get; set; } = ContactMessageStatus.New;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
