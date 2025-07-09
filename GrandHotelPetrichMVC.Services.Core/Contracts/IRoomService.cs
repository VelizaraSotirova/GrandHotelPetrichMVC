using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.ViewModels.Guests.Booking;

namespace GrandHotelPetrichMVC.Services.Core.Contracts
{
    public interface IRoomService
    {
        Task<IEnumerable<AvailableRoomViewModel>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests, RoomType? roomType);
        Task<RoomDetailsViewModel?> GetRoomDetailsAsync(Guid roomId);
        Task<bool> MarkRoomAsOccupiedAsync(Guid roomId);
    }
}
