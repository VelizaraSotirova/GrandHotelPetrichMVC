namespace GrandHotelPetrichMVC.ViewModels.Restaurant
{
    public class RestaurantViewModel
    {
        public string Title { get; set; } = "Restaurant & Dining";

        public string Description { get; set; } = "Experience exquisite cuisine in our elegant restaurant featuring both international and traditional Bulgarian dishes.";

        public List<MenuSection> MenuSections { get; set; } = new();

        public class MenuSection
        {
            public string Name { get; set; } = null!;
            public string Hours { get; set; } = null!;
            public List<MenuItem> Items { get; set; } = new();
        }

        public class MenuItem
        {
            public string Title { get; set; } = null!;
            public string Description { get; set; } = null!;
            public string Price { get; set; } = null!;
        }
    }
}
