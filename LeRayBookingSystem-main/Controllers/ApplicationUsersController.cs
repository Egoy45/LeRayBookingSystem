using LeRayBookingSystem.Data;
using LeRayBookingSystem.Models;
using LeRayBookingSystem.Services;
using LeRayBookingSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LeRayBookingSystem.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public ApplicationUsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: ApplicationUsers
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Select(u => new ApplicationUserViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Gender = u.Gender,
                    Address = u.Address,
                    Bio = u.Bio,
                    Occupation = u.Occupation,
                    SocialMediaLinks = u.SocialMediaLinks,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    ProfilePictureThumbnailUrl = u.ProfilePictureThumbnailUrl,
                    DateJoined = u.DateJoined
                })
                .ToListAsync();

            return View(users);
        }

        // GET: ApplicationUsers/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return View(MapToViewModel(user));
        }

        // GET: ApplicationUsers/Create
        public IActionResult Create() => View();

        // POST: ApplicationUsers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Ensure we have valid data
            var fullName = string.IsNullOrWhiteSpace(model.FullName) ? "Unnamed User" : model.FullName.Trim();
            var gender = string.IsNullOrWhiteSpace(model.Gender) ? "Unspecified" : model.Gender.Trim();
            var address = string.IsNullOrWhiteSpace(model.Address) ? "Not provided" : model.Address.Trim();

            // Generate a safe and unique email
            var baseEmail = Regex.Replace(fullName.ToLower(), @"\s+", "");
            var email = $"{baseEmail}@leray.com";

            int counter = 1;
            while (await _userManager.FindByEmailAsync(email) != null)
            {
                email = $"{baseEmail}{counter}@leray.com";
                counter++;
            }

            var user = new ApplicationUser(fullName, gender, address)
            {
                UserName = email,
                Email = email,
                Bio = model.Bio,
                Occupation = model.Occupation,
                SocialMediaLinks = model.SocialMediaLinks,
                ProfilePictureUrl = model.ProfilePictureUrl,
                ProfilePictureThumbnailUrl = model.ProfilePictureThumbnailUrl,
                DateJoined = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // For demo — generate a random strong password
            var password = $"User@{Guid.NewGuid().ToString("N")[..8]}";

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Assign default role (can adjust if needed)
                await _userManager.AddToRoleAsync(user, "Client");

                // Send welcome email
                await _emailService.SendEmailAsync(
                    user.Email!,
                    "Welcome to LeRay Booking System",
                    $@"<p>Hello {user.FullName},</p>
                       <p>Your account has been successfully created.</p>
                       <p>Login Email: {user.Email}</p>
                       <p>Temporary Password: {password}</p>
                       <p>Please change your password after first login.</p>
                       <p>Thank you,<br/>LeRay Team</p>");

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // GET: ApplicationUsers/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return View(MapToViewModel(user));
        }

        // POST: ApplicationUsers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUserViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.FullName = model.FullName ?? user.FullName;
            user.Gender = model.Gender ?? user.Gender;
            user.Address = model.Address ?? user.Address;
            user.Bio = model.Bio;
            user.Occupation = model.Occupation;
            user.SocialMediaLinks = model.SocialMediaLinks;
            user.ProfilePictureUrl = model.ProfilePictureUrl;
            user.ProfilePictureThumbnailUrl = model.ProfilePictureThumbnailUrl;
            user.DateJoined = model.DateJoined;
            user.UpdatedAt = DateTime.UtcNow;

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
                    return NotFound();
                throw;
            }
        }

        // GET: ApplicationUsers/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return View(MapToViewModel(user));
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id) =>
            _context.Users.Any(e => e.Id == id);

        private static ApplicationUserViewModel MapToViewModel(ApplicationUser user) => new()
        {
            Id = user.Id,
            FullName = user.FullName,
            Gender = user.Gender,
            Address = user.Address,
            Bio = user.Bio,
            Occupation = user.Occupation,
            SocialMediaLinks = user.SocialMediaLinks,
            ProfilePictureUrl = user.ProfilePictureUrl,
            ProfilePictureThumbnailUrl = user.ProfilePictureThumbnailUrl,
            DateJoined = user.DateJoined
        };
    }
}
