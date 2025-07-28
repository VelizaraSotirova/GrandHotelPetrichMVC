using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels.Admin.Payment;
using Microsoft.EntityFrameworkCore;

namespace GrandHotelPetrichMVC.Services.Core
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly ApplicationDbContext _context;

        public PaymentMethodService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentMethodViewModel>> GetAllMethodsAsync()
        {
            return await _context.PaymentMethods
                .OrderByDescending(pm => pm.IsActive)
                .ThenBy(pm => pm.Name)
                .Select(pm => new PaymentMethodViewModel
                {
                    Id = pm.Id,
                    Name = pm.Name,
                    IsActive = pm.IsActive
                })
                .ToListAsync();
        }

        public async Task<bool> AddMethodAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var exists = await _context.PaymentMethods
                .AnyAsync(pm => pm.Name == name);

            if (exists) return false;

            var method = new PaymentMethod
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                IsActive = true
            };

            _context.PaymentMethods.Add(method);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DisableMethodAsync(Guid id)
        {
            var method = await _context.PaymentMethods.FindAsync(id);
            if (method == null) return false;

            method.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReactivateMethodAsync(Guid id)
        {
            var method = await _context.PaymentMethods.FindAsync(id);
            if (method == null) return false;

            method.IsActive = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
