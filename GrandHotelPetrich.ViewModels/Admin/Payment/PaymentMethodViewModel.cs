using System.ComponentModel.DataAnnotations;
using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Booking;

namespace GrandHotelPetrichMVC.ViewModels.Admin.Payment
{
    public class PaymentMethodViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
