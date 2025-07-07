using GrandHotelPetrichMVC.GCommon.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Staff;

namespace GrandHotelPetrichMVC.Data.Models
{
    public class Staff
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(UserIdMaxLength)]
        public string UserId { get; set; } = null!; // Foreign key to the User table

        [Required]
        public StaffRole Role { get; set; } = StaffRole.Housekeeping;

        [Required]
        public StaffShifts Shift { get; set; } = StaffShifts.Day;

        [Required]
        public StaffStatus Status { get; set; } = StaffStatus.Active;

        [Required]
        public DateTime HireDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Salary { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        // Computed property for full name
        public string FullName => $"{User.FirstName} {User.LastName}";
    }
}
