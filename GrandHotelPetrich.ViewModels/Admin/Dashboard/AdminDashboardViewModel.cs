namespace GrandHotelPetrichMVC.ViewModels.Admin.Dashboard
{
    public class AdminDashboardViewModel
    {
        public int AvailableRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int TotalRooms => AvailableRooms + OccupiedRooms;

        public int StaffCount { get; set; }

        public decimal MonthlyRevenue { get; set; }
        public decimal PaidSalaries { get; set; }

        public DateTime CurrentMonth { get; set; } = DateTime.UtcNow;
    }
}
