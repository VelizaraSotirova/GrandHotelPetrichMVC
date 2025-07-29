using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using GrandHotelPetrichMVC.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GrandHotelPetrichMVC.Services.Core
{
    public class ContactService : IContactService
    {
        private readonly ApplicationDbContext _context;

        public ContactService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ContactMessageFormViewModel> GetPrefilledContactFormAsync(string? userEmail)
        {
            var model = new ContactMessageFormViewModel();

            if (!string.IsNullOrEmpty(userEmail))
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == userEmail);

                if (user != null)
                {
                    model.Name = $"{user.FirstName} {user.LastName}";
                    model.Email = user.Email!;
                    model.Phone = user.PhoneNumber;
                }
            }

            return model;
        }

        public async Task<bool> SubmitContactFormAsync(ContactMessageFormViewModel model)
        {
            var message = new ContactMessage
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.Phone,
                Subject = model.Subject,
                Message = model.Message,
            };

            _context.ContactMessages.Add(message);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
