using System.ComponentModel.DataAnnotations;

namespace LeRayBookingSystem.ViewsModels
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}