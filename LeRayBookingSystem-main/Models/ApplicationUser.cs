using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeRayBookingSystem.Models
{
    /// <summary>
    /// Represents an application user that extends the default IdentityUser
    /// with additional profile, audit, and relationship data.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // -------------------------
        // 🔹 Required Profile Fields
        // -------------------------
        [Required, MaxLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        // -------------------------
        // 🔹 Optional Profile Fields
        // -------------------------
        [MaxLength(500)]
        public string? Bio { get; set; }

        [MaxLength(100)]
        public string? Occupation { get; set; }

        [MaxLength(300)]
        [Display(Name = "Social Media Links")]
        public string? SocialMediaLinks { get; set; }

        [Display(Name = "Profile Picture URL")]
        public string? ProfilePictureUrl { get; set; }

        [Display(Name = "Alt Text")]
        public string? ProfilePictureAltText { get; set; }

        [Display(Name = "Thumbnail URL")]
        public string? ProfilePictureThumbnailUrl { get; set; }

        // -------------------------
        // 🔹 Audit & Activity Fields
        // -------------------------
        [Display(Name = "Date Joined")]
        public DateTime DateJoined { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Login")]
        public DateTime LastLogin { get; set; } = DateTime.UtcNow;

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // -------------------------
        // 🔹 Navigation Properties
        // -------------------------
        public ICollection<Appointment>? Appointments { get; set; }

        [InverseProperty(nameof(Appointment.CreatedByUser))]
        public ICollection<Appointment>? CreatedAppointments { get; set; }

        [InverseProperty(nameof(Appointment.UpdatedByUser))]
        public ICollection<Appointment>? UpdatedAppointments { get; set; }

        [InverseProperty(nameof(Appointment.LastUpdatedByUser))]
        public ICollection<Appointment>? LastUpdatedAppointments { get; set; }

        // -------------------------
        // 🔹 Constructors
        // -------------------------

        /// <summary>
        /// Main constructor enforcing required fields.
        /// </summary>
        public ApplicationUser(string fullName, string gender, string address)
        {
            // ✅ Automatically generate a unique User ID
            Id = GenerateCustomUserId();

            FullName = fullName;
            Gender = gender;
            Address = address;
            DateJoined = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Parameterless constructor for EF Core and Identity.
        /// </summary>
        public ApplicationUser()
        {
            // ✅ Also generate ID for Identity's internal use
            Id = GenerateCustomUserId();

            FullName = string.Empty;
            Gender = string.Empty;
            Address = string.Empty;
        }

        // -------------------------
        // 🔹 Custom ID Generator
        // -------------------------
        private static string GenerateCustomUserId()
        {
            return $"USR-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";
        }
    }
}
