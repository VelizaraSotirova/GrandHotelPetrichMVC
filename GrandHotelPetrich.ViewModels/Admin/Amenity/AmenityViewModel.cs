namespace GrandHotelPetrichMVC.ViewModels.Admin.Amenity
{
    public class AmenityViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
