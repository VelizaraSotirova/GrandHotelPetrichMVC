namespace GrandHotelPetrichMVC.GCommon
{
    public class ValidationConstants
    {
        public static class Booking
        {
            public const int MaxLengthUserId = 450;
            public const int SpecialRequestsMaxLength = 1000;
            public const int PaymentMethodMaxLength = 30;
            public const int CheckInNotesMaxLength = 500;
            public const int CheckOutNotesMaxLength = 500;

            public const int MinNumberOfGuests = 1; 
            public const int MaxNumberOfGuests = 6;
        }

        public static class ContactMessage
        {
            public const int NameMaxLength = 255;
            public const int EmailMaxLength = 255;
            public const int PhoneNumberMaxLength = 20;
            public const int SubjectMaxLength = 200;
            public const int MessageMaxLength = 2000;
        }

        public static class Gallery
        {
            public const int ImageUrlMaxLength = 500;
            public const int TitleMaxLength = 200;
            public const int DescriptionMaxLength = 1000;
            public const int CategoryMaxLength = 10; // rooms, dining, amenities, spa, lobby
        }

        public static class Revenue
        {
            public const int RevenueSourceNameMaxLength = 50;
            public const int DescriptionMaxLength = 200;
        }

        public static class Review
        {
            public const int UserIdMaxLength = 450;
            public const int TitleMaxLength = 100;
            public const int CommentMaxLength = 2000;
            public const int StatusMaxLength = 10; // Pending, Approved, Rejected

            public const int MinRating = 1;
            public const int MaxRating = 5;
        }

        public static class Room
        {
            public const int MaxNameLength = 100;
            public const int RoomTypeMaxLength = 50; // Standard, Deluxe, Suite, etc.
            public const int BedConfigMaxLength = 100; // e.g., "King + Single" 
            public const int MaxDescriptionLength = 1000;
            public const int ImageUrlMaxLength = 200;
            public const int AmentiesMaxLength = 2000; // JSON array as string
            public const int BadgeMaxLength = 100;

            public const int PricePerNightMin = 1;
            public const int PricePerNightMax = 10000;

            public const int MaxConstCapacity = 20; // Maximum number of guests
            public const int MinCapacity = 1;
        }

        public static class StatusOfRoom
        {
            public const int MaxNotesLength = 500;
        }

        public static class User
        {
            public const int FirstNameMaxLength = 50;
            public const int LastNameMaxLength = 50;
            public const int AddressMaxLength = 500;
            public const int RoleMaxLength = 20; // Customer, Admin, Staff
        }

        public static class Staff
        {
            public const int UserIdMaxLength = 450;

            public const int StaffMinSalary = 600;
            public const int StaffMaxSalary = 10000;
        }

        public static class Amenity
        {
            public const int NameMaxLength = 100;
        }
    }
}
