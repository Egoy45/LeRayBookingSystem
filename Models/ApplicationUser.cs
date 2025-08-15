using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LeRayBookingSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Required fields - enforced via constructor (not C# 11 'required' keyword)
        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required, MaxLength(10)]
        public string Gender { get; set; }

        [Required, MaxLength(200)]
        public string Address { get; set; }

        // Optional fields
        public string? Bio { get; set; }
        public string? Occupation { get; set; }
        public string? SocialMediaLinks { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? ProfilePictureAltText { get; set; }
        public string? ProfilePictureThumbnailUrl { get; set; }

        public DateTime DateJoined { get; set; } = DateTime.UtcNow;
        public DateTime LastLogin { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Constructor with required fields
        public ApplicationUser(string fullName, string gender, string address)
        {
            FullName = fullName;
            Gender = gender;
            Address = address;
        }

        // Parameterless constructor required by EF Core and Identity
        public ApplicationUser()
        {
            // Assign null-forgiving values to satisfy compiler (CS8618)
            FullName = null!;
            Gender = null!;
            Address = null!;
        }
    }
}
