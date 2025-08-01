# GrandHotelPetrichMVC
Final Project - a hotel management system

📚 Overview
GrandHotelPetrichMVC is a multi-area ASP.NET Core MVC application for managing hotel operations. It includes support for:
Guest room booking
Receptionist real-time bookings
Admin staff/room/gallery/revenue control
Role-based access using ASP.NET Core Identity
The application follows an MVC architecture with a layered service approach and Entity Framework Core for data access.


🧱 Project Structure
GrandHotelPetrichMVC/
│
├── Areas/
│   ├── Guest/
│   ├── Receptionist/
│   ├── Admin/
│
├── Controllers/
├── ViewModels/
├── Data/
│   ├── Models/
│   ├── ApplicationDbContext.cs
│
├── Services/
│   ├── Core/
│   └── Core/Contracts/
│
├── wwwroot/
├── Views/
├── Program.cs
├── Startup.cs (if present)
└── appsettings.json


🧪 Unit Testing
All services are covered with NUnit-based test classes.
Use Moq for mocking UserManager<T>.
Coverage: ~100% on business-critical services.

Example test classes:
ReceptionistTests.cs
RoomAdminTests.cs
StaffTests.cs


🔐 Authentication & Authorization
ASP.NET Core Identity is used with custom User : IdentityUser.
Roles include: Admin, Receptionist, Housekeeping, Customer.
Role checks via:
[Authorize(Roles = "Admin")]


🧠 Services Overview
Each service resides in Services.Core and implements a contract interface in Services.Core.Contracts.
IBookingService / BookingService
Handles customer room bookings.
Filters available rooms, calculates totals, handles booking logic.

IReceptionistService / ReceptionistService
Real-time room search and booking for walk-in guests.
Manages cleaning workflow:
Rooms go to OutForCleaning after checkout.
MarkRoomAsCleaned brings them back to Available.

IStaffService / StaffService
Manages hotel staff CRUD operations.
Assigns Identity roles and updates StaffStatus.

IRoomAdminService / RoomAdminService
Room management for admins.
Supports adding/editing rooms, amenities, toggling status/active.

Other Services:
IGalleryService, IRevenueService, IContactService are present for other admin functionalities.


🛏 Room Lifecycle Logic
Room booked → status may become Occupied (if current date is in range).
After checkout → status is updated to OutForCleaning.
Receptionist manually marks it Available after cleaning.

Logic handled in:
ReceptionistService.UpdateRoomsThatNeedCleaningAsync()
ReceptionistService.MarkRoomAsCleanedAsync()


💾 Database Design (Simplified)
Main Entities
User (inherits from IdentityUser)
Booking
Room, RoomStatus, RoomAmenity
Staff
Revenue
PaymentMethod
Amenity, ContactMessage, Review, GalleryImage

Relationships
One Room ↔ Many Bookings
One Room ↔ One RoomStatus
Many-to-many between Room and Amenity via RoomAmenity
One User ↔ One Staff (if employee)


🧾 Admin Area
Features
Staff filtering by status
Assign roles and salary
View all bookings, revenues, and feedback
Manage:
Rooms
Amenities
Payment Methods
Gallery images
Contact messages
Reviews


🎯 Booking Flow (Guest Area)
Guest chooses:
Check-in/out dates
Room type & number of guests

System displays available rooms
Guest selects a room, enters data
Chooses payment method
Confirmation + success screen

Handled via:
BookingController + BookingService
ViewModels: BookingFormViewModel, BookingSuccessViewModel, etc.


🧪 Testing Highlights - Services Coverage: ✅ 92.2%
Coverage:
AdminService: ✅ 100%
AmenityService: ✅ 100%
BookingService: ✅ 92.7%
ContactService: ✅ 100%
GalleryService: ✅ 83.8%
PaymentMethodService: ✅ 100%
ReceptionistService: ✅ 94.8%
ReviewService: ✅ 100%
RoomAdminService: ✅ 100%
StaffService: ✅ 70%
ProfileService: ✅ 100%


⚙️ Configuration
appsettings.json: DB connection strings, Identity config

EF Core used with migrations:
dotnet ef migrations add Init
dotnet ef database update


💡 Development Tips
Use Areas for feature segregation.
Keep all business logic inside services—controllers are thin.
Apply [Authorize] wisely per controller or action.
Use ViewModels to shape view-facing data.


📌 Final Notes
Identity Role Management is handled via UserManager + RoleManager APIs.
Date comparisons are done with DateTime.UtcNow.Date.
Revenue is tracked via the Revenue table with RevenueSource.




🎨 Frontend Development Guide – GrandHotelPetrichMVC
🧭 Overview
The frontend is built with ASP.NET Core MVC Razor Views and uses:
HTML/CSS/Taiwand CSS for layout & responsiveness
Razor View Engine for rendering dynamic content
JavaScript (optional & light) for interactivity
Areas-based routing for modular UI


🧱 Razor Layout Structure
The shared layout is typically in:
Views/Shared/_Layout.cshtml
Contains:
<head> with dynamic title
Top navigation bar
@RenderBody() for page injection
Footer and optional modals

Custom layouts exist in:
Areas/Guest/Views/Shared/
Areas/Admin/Views/Shared/
Areas/Receptionist/Views/Shared/


🧩 Partial Views
Reusable HTML components:
_BookingFormPartial.cshtml
_Navbar.cshtml


🚦Routing & Areas
Front-end routing uses MVC + Areas:
/ -> HomeController -> Landing page
/Guest/Booking -> Customer booking flow
/Receptionist/Booking -> Staff booking
/Admin/Staff -> Staff management

Set area-aware links like:
asp-area="Admin" asp-controller="Staff" asp-action="Index"


📆 Booking UI Flow (Guest)
Index.cshtml – Booking form with check-in/check-out + guest count
Room options displayed below form (via same view)
Confirm.cshtml – Review & confirm booking
Success.cshtml – Confirmation screen


📸 Admin UI Panels
Admin dashboard views are organized:
/Admin/Rooms/
/Admin/Staff/
/Admin/Revenue/
/Admin/Gallery/

Each feature has:
Index.cshtml (list/filter)
Create.cshtml / Edit.cshtml (forms)


🧼 Clean Markup Guidelines
Use semantic tags: <section>, <header>, <footer>
Always bind models to forms using asp-for
All forms use anti-forgery: @Html.AntiForgeryToken()
Use TempData["Success"] / TempData["Error"] for status messaging


📸 Gallery Upload UI
Admin can upload photos via:
Gallery/Upload.cshtml form
File is uploaded using IFormFile


💡 UX Considerations
Mobile-first responsive design using Bootstrap 5
Role-based UI filtering (non-admins can’t see admin links)
Clean navigation with active link highlights
Flash messages after important actions (save/delete/book)


🧭 Future Improvements
Add AJAX room filtering with JS or jQuery
Add chart dashboard with Chart.js for admin
Add modal previews for gallery items
Add dark mode toggle with Tailwind/Bootstrap switch
