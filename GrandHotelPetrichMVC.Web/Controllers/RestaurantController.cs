using GrandHotelPetrichMVC.ViewModels.Restaurant;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotelPetrichMVC.Web.Controllers
{
    public class RestaurantController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new RestaurantViewModel
            {
                MenuSections = new List<RestaurantViewModel.MenuSection>
                {
                    new RestaurantViewModel.MenuSection
                    {
                        Name = "Breakfast Buffet",
                        Hours = "6:00 AM - 10:30 AM",
                        Items = new List<RestaurantViewModel.MenuItem>
                        {
                            new RestaurantViewModel.MenuItem
                            {
                                Title = "Continental Breakfast",
                                Description = "Fresh pastries, fruits, yogurt, and cereals",
                                Price = "€15"
                            },
                            new RestaurantViewModel.MenuItem
                            {
                                Title = "Bulgarian Breakfast",
                                Description = "Traditional banitsa, kashkaval, tomatoes, and cucumbers",
                                Price = "€18"
                            }
                        }
                    },
                    new RestaurantViewModel.MenuSection
                    {
                        Name = "Lunch & Dinner",
                        Hours = "12:00 PM - 10:00 PM",
                        Items = new List<RestaurantViewModel.MenuItem>
                        {
                            new RestaurantViewModel.MenuItem
                            {
                                Title = "Grilled Salmon",
                                Description = "Atlantic salmon with seasonal vegetables and lemon sauce",
                                Price = "€28"
                            },
                            new RestaurantViewModel.MenuItem
                            {
                                Title = "Bulgarian Lamb",
                                Description = "Slow-cooked lamb with traditional herbs and red wine",
                                Price = "€32"
                            }
                        }
                    }
                }
            };

            return View(viewModel);
        }
    }
}
