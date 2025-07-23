using GrandHotelPetrichMVC.Data;
using GrandHotelPetrichMVC.Data.DataSeed;
using GrandHotelPetrichMVC.Data.Models;
using GrandHotelPetrichMVC.Services;
using GrandHotelPetrichMVC.Services.Core;
using GrandHotelPetrichMVC.Services.Core.Contracts;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace GrandHotelPetrichMVC.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<User, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure the login redirect path
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login"; // default login path from scaffolding
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IStaffService, StaffService>();
            builder.Services.AddScoped<IReceptionistService, ReceptionistService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();


            builder.Services.AddSingleton<IEmailSender, DummyEmailSender>();



            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Seed the database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    DataSeed.Initialize(context);
                    // Uncomment the following lines if the data does not seed in database
                    // Make the methods in DataSeed.cs public 
                    //DataSeed.SeedPaymentMethods(context); 
                    //DataSeed.SeedRevenueSources(context);
                    //DataSeed.SeedReceptionist(context);

                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    // START: Assign role to user
                    // Create roles if they don't exist
                    var roles = new[] { "Admin", "Customer", "Receptionist", "Manager" };
                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                    }

                    // Assign users to roles
                    var adminUser = await userManager.FindByEmailAsync("admin@hotel.com");
                    if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }

                    var receptionistUser = await userManager.FindByEmailAsync("receptionist@hotel.com");
                    if (receptionistUser != null && !await userManager.IsInRoleAsync(receptionistUser, "Receptionist"))
                    {
                        await userManager.AddToRoleAsync(receptionistUser, "Receptionist");
                    }

                    var customerUser = await userManager.FindByEmailAsync("john@customer.com");
                    if (customerUser != null && !await userManager.IsInRoleAsync(customerUser, "Customer"))
                    {
                        await userManager.AddToRoleAsync(customerUser, "Customer");
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Custom routes for sign-in and sign-up
            app.MapGet("/SignIn", context =>
            {
                context.Response.Redirect("/Identity/Account/Login");
                return Task.CompletedTask;
            });

            app.MapGet("/SignUp", context =>
            {
                context.Response.Redirect("/Identity/Account/Register");
                return Task.CompletedTask;
            });


            app.MapControllerRoute(
                name: "area",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapDefaultControllerRoute();
            app.MapRazorPages();

            app.Run();
        }
    }
}
