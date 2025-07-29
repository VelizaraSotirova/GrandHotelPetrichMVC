using GrandHotelPetrichMVC.ViewModels;

namespace GrandHotelPetrichMVC.Services.Core.Contracts
{
    public interface IContactService
    {
        Task<ContactMessageFormViewModel> GetPrefilledContactFormAsync(string? userEmail);
        Task<bool> SubmitContactFormAsync(ContactMessageFormViewModel model);
    }
}
