using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.ViewModels.Guests.Booking;
using GrandHotelPetrichMVC.ViewModels.Receptionists.Booking;
using GrandHotelPetrichMVC.ViewModels.Receptionists.Room;

namespace GrandHotelPetrichMVC.Services.Core.Contracts
{
    public interface IReceptionistService
    {
        Task<List<AvailableRoomViewModel>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests, RoomType? roomType);
        Task<List<PaymentMethodViewModel>> GetPaymentMethodsAsync();
        Task<ReceptionistBookingSuccessViewModel?> CreateBookingAsync(ReceptionistBookingCreateViewModel model);

        Task<List<RoomStatusViewModel>> GetRoomsOutForCleaningAsync();
        Task<bool> MarkRoomAsCleanedAsync(Guid roomId);
        Task UpdateRoomsThatNeedCleaningAsync();

    }
}
