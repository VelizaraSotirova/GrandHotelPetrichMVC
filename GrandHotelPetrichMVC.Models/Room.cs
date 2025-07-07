using GrandHotelPetrichMVC.GCommon.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static GrandHotelPetrichMVC.GCommon.ValidationConstants.Room;

namespace GrandHotelPetrichMVC.Data.Models
{
    public class Room
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; } = string.Empty;

        public RoomType RoomType { get; set; } = RoomType.Standard;

        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerNight { get; set; }

        public int MaxCapacity { get; set; }

        [Required]
        public BedConfiguration BedConfiguration { get; set; } = BedConfiguration.Single;

        [MaxLength(MaxDescriptionLength)]
        public string Description { get; set; } = null!;

        [MaxLength(ImageUrlMaxLength)]
        public string ImageUrl { get; set; } = null!;

        public virtual ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();

        public RoomBadge Badge { get; set; } = RoomBadge.None;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
