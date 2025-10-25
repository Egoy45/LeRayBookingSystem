using System.ComponentModel.DataAnnotations;

namespace LeRayBookingSystem.ViewModels
{
    public class ApplicationUserViewModel
    {
        [Required]
        public required string Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public required string FullName { get; set; }

        [Required]
        public required string Gender { get; set; }

        [Required]
        public required string Address { get; set; }

        public string? Bio { get; set; }

        public string? Occupation { get; set; }

        [Display(Name = "Social Media")]
        public string? SocialMediaLinks { get; set; }

        [Display(Name = "Profile Picture")]
        public string? ProfilePictureUrl { get; set; }

        public string? ProfilePictureThumbnailUrl { get; set; }

        [Display(Name = "Date Joined")]
        public DateTime DateJoined { get; set; } = DateTime.Now;
    }
}
