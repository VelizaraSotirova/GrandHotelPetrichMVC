using System.ComponentModel.DataAnnotations;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Room;

namespace GrandHotelPetrichMVC.Data.Models
{
    public class Amenity
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(AmentiesMaxLength)]
        public string Name { get; set; } = string.Empty; // e.g. "Wi-Fi"

        public virtual ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
    }
}
