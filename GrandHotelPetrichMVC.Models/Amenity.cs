using System.ComponentModel.DataAnnotations;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Room;

namespace GrandHotelPetrichMVC.Data.Models
{
    public class Amenity
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(AmentiesMaxLength)]
        public string Name { get; set; } = null!; 

        public bool IsActive { get; set; } = true; // Indicates if the amenity is currently available

        public virtual ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
    }
}
