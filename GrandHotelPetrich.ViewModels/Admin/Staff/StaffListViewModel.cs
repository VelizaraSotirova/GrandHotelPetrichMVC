using GrandHotelPetrichMVC.GCommon.Enums;

namespace GrandHotelPetrichMVC.ViewModels.Admin.Staff
{
    public class StaffListViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public StaffRole Role { get; set; }
        public StaffShifts Shift { get; set; }
        public StaffStatus Status { get; set; }
        public string PhoneNumber { get; set; } = null!;
    }
}
