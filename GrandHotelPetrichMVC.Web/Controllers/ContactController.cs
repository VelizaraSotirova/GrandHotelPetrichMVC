using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
