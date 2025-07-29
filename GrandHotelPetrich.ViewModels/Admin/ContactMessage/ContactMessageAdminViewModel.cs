namespace GrandHotelPetrichMVC.ViewModels.Admin.ContactMessage
{
    public class ContactMessageAdminViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Subject { get; set; } = null!;

        public string Message { get; set; } = null!;

        public string Status { get; set; } = null!;

        public DateTime ReceivedAt { get; set; }
    }
}
