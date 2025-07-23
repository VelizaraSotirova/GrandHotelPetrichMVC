using GrandHotelPetrichMVC.Services.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Controllers
{
    public class RoomsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
