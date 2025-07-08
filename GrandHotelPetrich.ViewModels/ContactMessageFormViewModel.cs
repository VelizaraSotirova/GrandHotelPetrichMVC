using System.ComponentModel.DataAnnotations;

namespace GrandHotelPetrichMVC.ViewModels
{
    public class ContactMessageFormViewModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        public string? Phone { get; set; }

        [Required]
        public string Subject { get; set; } = null!;

        [Required]
        public string Message { get; set; } = null!;

    }
}
