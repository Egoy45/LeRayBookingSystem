using LeRayBookingSystem.Models.DTOs;
using LeRayBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LeRayBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountApiController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager) : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        // ---------- REGISTER ----------
        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exists = await _userManager.FindByEmailAsync(model.Email);
            if (exists != null) return Conflict(new { message = "Email already registered." });

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = $"{model.FirstName} {model.LastName}",
                DateJoined = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(new { message = "Registration failed.", errors = result.Errors.Select(e => e.Description) });

            if (!await _roleManager.RoleExistsAsync("Client"))
                await _roleManager.CreateAsync(new IdentityRole("Client"));

            await _userManager.AddToRoleAsync(user, "Client");

            return Ok(new { message = "Registration successful." });
        }

        // ---------- LOGIN (ISSUES IDENTITY COOKIE) ----------
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (string.IsNullOrWhiteSpace(model.EmailOrUsername) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest(new { message = "Email/Username and Password are required." });

            var user = await _userManager.FindByEmailAsync(model.EmailOrUsername)
                    ?? await _userManager.FindByNameAsync(model.EmailOrUsername);

            if (user == null) return Unauthorized(new { message = "Invalid credentials." });

            // Clear any previous sign-in
            await _signInManager.SignOutAsync();

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded) return Unauthorized(new { message = "Invalid credentials." });

            // Explicit sign-in to ensure cookie is set
            await _signInManager.SignInAsync(user, isPersistent: false);

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                message = "Login successful.",
                redirectUrl = "/Home/Booking",
                user = new { email = user.Email, name = user.FullName, roles }
            });
        }

        // ---------- LOGOUT ----------
        [Authorize(AuthenticationSchemes = "Identity.Application")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully." });
        }
    }
}
