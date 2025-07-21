using GrandHotelPetrichMVC.GCommon.Enums;
using System.ComponentModel.DataAnnotations;

namespace GrandHotelPetrichMVC.ViewModels.Admin.Staff
{
    public class CreateStaffViewModel
    {
        [Required]
        public string UserEmail { get; set; } = string.Empty;

        [Required]
        public StaffRole Role { get; set; }

        [Required]
        public StaffShifts Shift { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }
    }

}
