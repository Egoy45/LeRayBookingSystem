using LeRayBookingSystem.Data;
using LeRayBookingSystem.Models;
using LeRayBookingSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------
// Add Services
// ------------------------------

// Add DbContext with connection string from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// Identity with Email Confirmation & 2FA
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true; // Require email confirmation
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider; // 2FA
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Email Service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IEmailSender, EmailService>();

// SMS Service
builder.Services.AddScoped<ISmsService, SmsService>();

// Admin Notification Service
builder.Services.AddScoped<AdminNotificationService>();

// MVC & Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ------------------------------
// Build App
// ------------------------------
var app = builder.Build();

// ------------------------------
// Seed Roles & SuperAdmin
// ------------------------------
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Client", "Admin", "SuperAdmin" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

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
                fullName: superAdminFullName ?? "Super Admin",
                gender: superAdminGender ?? "Not Specified",
                address: superAdminAddress ?? "Not Specified")
            {
                UserName = superAdminEmail,
                Email = superAdminEmail,
                EmailConfirmed = true,
                DateJoined = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(newSuperAdmin, superAdminPassword ?? "StrongP@ssword123");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(newSuperAdmin, "SuperAdmin");
        }
    }
}

// ------------------------------
// Middleware
// ------------------------------
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

// Identity & MVC routing
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
