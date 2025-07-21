namespace GrandHotelPetrichMVC.ViewModels.Admin.ContactMessage
{
    public class ContactMessageAdminViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime ReceivedAt { get; set; }
    }
}
