using LeRayBookingSystem.Models;
using LeRayBookingSystem.Services; // ✅ Added for AuditLogService
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeRayBookingSystem.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly AuditLogService _auditService; // ✅ Added

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginModel> logger,
            AuditLogService auditService) // ✅ Injected
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _auditService = auditService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
            {
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // ✅ Log successful login
                    await _auditService.LogAsync(
                        "Account",
                        "Login",
                        user.Id,
                        $"User '{user.Email}' logged in successfully."
                    );

                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Admin") || roles.Contains("SuperAdmin"))
                    {
                        _logger.LogInformation("Admin logged in — redirecting to Dashboard.");
                        return LocalRedirect("~/Admin/Dashboard");
                    }

                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(ReturnUrl);
                }
                else
                {
                    // ❌ Log failed login
                    await _auditService.LogAsync(
                        "Account",
                        "LoginFailed",
                        user.Id,
                        $"Failed login attempt for '{user.Email}'."
                    );
                }
            }
            else
            {
                // ❌ Log invalid email attempt
                await _auditService.LogAsync(
                    "Account",
                    "LoginFailed",
                    "Unknown",
                    $"Failed login attempt for non-existent email '{Input.Email}'."
                );
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return Page();
        }
    }
}
