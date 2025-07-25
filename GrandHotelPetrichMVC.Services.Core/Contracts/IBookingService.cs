using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.ViewModels.Guests.Booking;

namespace GrandHotelPetrichMVC.Services.Core.Contracts
{
    public interface IBookingService
    {
        Task<IEnumerable<AvailableRoomViewModel>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests, RoomType? roomType);
        Task<RoomDetailsViewModel?> GetRoomDetailsAsync(Guid roomId);
        //Task<bool> MarkRoomAsOccupiedAsync(Guid roomId);
        Task<BookingConfirmationViewModel> PrepareBookingConfirmationAsync(Guid roomId, DateTime checkIn, DateTime checkOut, int guests);
        Task<Guid> ConfirmBookingAsync(BookingConfirmationViewModel model, string userId);
        Task<List<PaymentMethodViewModel>> GetPaymentMethodsAsync();

        Task<BookingSuccessViewModel?> GetBookingSuccessAsync(Guid bookingId, string userId);

        Task<MyBookingsViewModel> GetBookingsForUserAsync(string userId, string filter);
    }
}
