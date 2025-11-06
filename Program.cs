using LeRayBookingSystem.Data;
using LeRayBookingSystem.Models;
using LeRayBookingSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ====================================
// SERVICES CONFIGURATION
// ====================================

// ✅ Email Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddTransient<IEmailSender, EmailService>();

// ✅ MVC + Razor
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ✅ JWT Token Generator
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// ✅ Bind JWT Settings
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);
builder.Services.AddScoped<IAuthService, AuthService>();

// ✅ Configure MySQL Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// ====================================
// IDENTITY CONFIGURATION
// ====================================

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ✅ Configure Identity Cookie (used by MVC)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Home/Index";
    options.AccessDeniedPath = "/Home/Index";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

// ====================================
// JWT AUTHENTICATION CONFIGURATION
// ====================================

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.Secret))
    throw new InvalidOperationException("JWT Secret not configured properly in appsettings.json.");

var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

// ✅ Add ONLY the JWT bearer scheme (do not re-register Identity cookie)
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// ====================================
// EMAIL + CORS CONFIGURATION
// ====================================

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendAccess", policy =>
    {
        policy.WithOrigins("http://localhost:5078")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// ====================================
// DATABASE INITIALIZATION (Roles + SuperAdmin)
// ====================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var config = services.GetRequiredService<IConfiguration>();

    string[] roleNames = { "Client", "Admin", "SuperAdmin" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
            Console.WriteLine($"✅ Role '{roleName}' created.");
        }
    }

    // === SUPERADMIN CREATION ===
    var superAdminEmail = config["SuperAdmin:Email"];
    var superAdminPassword = config["SuperAdmin:Password"];
    var superAdminFullName = config["SuperAdmin:FullName"];
    var superAdminGender = config["SuperAdmin:Gender"];
    var superAdminAddress = config["SuperAdmin:Address"];

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

            var result = await userManager.CreateAsync(newSuperAdmin, superAdminPassword ?? "StrongP@ssword123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newSuperAdmin, "SuperAdmin");
                Console.WriteLine($"✅ SuperAdmin '{superAdminEmail}' created.");
            }
            else
            {
                Console.WriteLine($"❌ Failed to create SuperAdmin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            Console.WriteLine($"ℹ️ SuperAdmin '{superAdminEmail}' already exists.");
        }
    }
    else
    {
        Console.WriteLine("⚠️ SuperAdmin email not set in appsettings.json");
    }
}

// ====================================
// MIDDLEWARE CONFIGURATION
// ====================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowFrontendAccess");

app.UseAuthentication();
app.UseAuthorization();

// ====================================
// ROLE-BASED DASHBOARD REDIRECTS
// ====================================

app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true && context.Request.Path == "/")
    {
        var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.GetUserAsync(context.User);

        if (user != null)
        {
            var roles = await userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
            {
                context.Response.Redirect("/Admin/Dashboard");
                return;
            }

            if (roles.Contains("SuperAdmin"))
            {
                context.Response.Redirect("/SuperAdmin/Dashboard");
                return;
            }
        }
    }

    await next();
});

// ====================================
// ROUTING CONFIGURATION
// ====================================

app.MapControllers();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
