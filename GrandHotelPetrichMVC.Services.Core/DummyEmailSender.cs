using Microsoft.AspNetCore.Identity.UI.Services;

namespace GrandHotelPetrichMVC.Services.Core
{
    public class DummyEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Log to console or ignore
            Console.WriteLine($"[MockEmail] To: {email}, Subject: {subject}");
            return Task.CompletedTask;
        }
    }
}
