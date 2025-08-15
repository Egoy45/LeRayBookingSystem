using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LeRayBookingSystem.Data;
using LeRayBookingSystem.Models;
using LeRayBookingSystem.Services; // ✅ For IEmailService & EmailService

var builder = WebApplication.CreateBuilder(args);

// ✅ Register Email Service so DI can resolve it
builder.Services.AddScoped<IEmailService, EmailService>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// ✅ SEED ROLES & SUPER ADMIN
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roleNames = { "Client", "Admin", "SuperAdmin" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
                Console.WriteLine($"✅ Role '{roleName}' created.");
            else
                Console.WriteLine($"❌ Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
        else
        {
            Console.WriteLine($"ℹ️ Role '{roleName}' already exists.");
        }
    }

    // Get SuperAdmin details from config
    var superAdminEmail = builder.Configuration["SuperAdmin:Email"];
    var superAdminPassword = builder.Configuration["SuperAdmin:Password"];
    var superAdminFullName = builder.Configuration["SuperAdmin:FullName"];
    var superAdminGender = builder.Configuration["SuperAdmin:Gender"];
    var superAdminAddress = builder.Configuration["SuperAdmin:Address"];

    if (!string.IsNullOrWhiteSpace(superAdminEmail))
    {
        var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);

        if (superAdminUser == null)
        {
            var newSuperAdmin = new ApplicationUser(
                fullName: superAdminFullName ?? "Default SuperAdmin",
                gender: superAdminGender ?? "Not Specified",
                address: superAdminAddress ?? "Not Specified")
            {
                UserName = superAdminEmail,
                Email = superAdminEmail,
                EmailConfirmed = true,
                DateJoined = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            var createUser = await userManager.CreateAsync(newSuperAdmin, superAdminPassword ?? "StrongP@ssword123");

            if (createUser.Succeeded)
            {
                await userManager.AddToRoleAsync(newSuperAdmin, "SuperAdmin");
                Console.WriteLine($"✅ SuperAdmin account '{superAdminEmail}' created and assigned role.");
            }
            else
            {
                Console.WriteLine($"❌ Failed to create SuperAdmin: {string.Join(", ", createUser.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            Console.WriteLine($"ℹ️ SuperAdmin '{superAdminEmail}' already exists.");
        }
    }
    else
    {
        Console.WriteLine("⚠️ SuperAdmin email is not set in appsettings.json.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
