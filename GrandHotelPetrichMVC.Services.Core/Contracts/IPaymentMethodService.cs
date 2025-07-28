using GrandHotelPetrichMVC.ViewModels.Admin.Payment;

namespace GrandHotelPetrichMVC.Services.Core.Contracts
{
    public interface IPaymentMethodService
    {
        Task<List<PaymentMethodViewModel>> GetAllMethodsAsync();
        Task<bool> AddMethodAsync(string name);
        Task<bool> DisableMethodAsync(Guid id);
        Task<bool> ReactivateMethodAsync(Guid id);
    }
}
