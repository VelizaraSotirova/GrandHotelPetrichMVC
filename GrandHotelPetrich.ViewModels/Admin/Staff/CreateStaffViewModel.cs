using GrandHotelPetrichMVC.GCommon.Enums;
using System.ComponentModel.DataAnnotations;

namespace GrandHotelPetrichMVC.ViewModels.Admin.Staff
{
    public class CreateStaffViewModel
    {
        [Required]
        public string UserEmail { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public StaffRole Role { get; set; }

        [Required]
        public StaffShifts Shift { get; set; }

        [Required]
        public StaffStatus Status { get; set; }

        [Range(600, 10000)]
        public decimal Salary { get; set; }
    }
}
