using GrandHotelPetrichMVC.GCommon.Enums;

namespace GrandHotelPetrichMVC.ViewModels.Admin.Staff
{
    public class StaffListViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public StaffRole Role { get; set; }
        public StaffShifts Shift { get; set; }
        public StaffStatus Status { get; set; }
        public string PhoneNumber { get; set; } = null!;
    }
}
