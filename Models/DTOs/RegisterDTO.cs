using System.ComponentModel.DataAnnotations;

namespace LeRayBookingSystem.Models.DTOs
{
    public class RegisterDto
    {
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        [Required, EmailAddress]
        public required string Email{ get; set; }
        [Required]
        public required string Password { get; set; }
        [Required, Compare("Password")]
        public required string ConfirmPassword { get; set; }
        [Required]
        public required string CaptchaToken { get; set; } // 
    }
}