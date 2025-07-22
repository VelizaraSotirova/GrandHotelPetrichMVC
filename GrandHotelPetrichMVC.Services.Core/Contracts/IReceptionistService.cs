using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.ViewModels.Guests.Booking;
using GrandHotelPetrichMVC.ViewModels.Receptionists.Booking;

namespace GrandHotelPetrichMVC.Services.Core.Contracts
{
    public interface IReceptionistService
    {
        Task<List<AvailableRoomViewModel>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests, RoomType? roomType);
        Task<List<PaymentMethodViewModel>> GetPaymentMethodsAsync();
        Task<ReceptionistBookingSuccessViewModel?> CreateBookingAsync(ReceptionistBookingCreateViewModel model);
    }

}
