using GrandHotelPetrichMVC.GCommon.Enums;
using GrandHotelPetrichMVC.ViewModels.Admin.Room;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GrandHotelPetrichMVC.Services.Core.Contracts
{
    public interface IRoomAdminService
    {
        Task<List<RoomListViewModel>> GetAllRoomsAsync();

        Task<RoomCreateViewModel?> GetRoomByIdAsync(Guid id);
        Task<bool> CreateRoomAsync(RoomCreateViewModel model);
        Task<bool> UpdateRoomAsync(RoomCreateViewModel model);
        Task<bool> ChangeRoomStatusAsync(Guid roomId, RoomStatus newStatus);
        Task<bool> ToggleRoomActiveAsync(Guid roomId, bool isActive);

        Task<RoomCreateViewModel> GetRoomCreateViewModelAsync();
        Task<List<SelectListItem>> GetAmenitiesSelectListAsync();

    }
}
