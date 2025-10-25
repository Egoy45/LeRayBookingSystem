using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeRayBookingSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        // --- Required Fields ---
        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required, MaxLength(10)]
        public string Gender { get; set; }

        [Required, MaxLength(200)]
        public string Address { get; set; }

        // --- Optional Profile Fields ---
        public string? Bio { get; set; }
        public string? Occupation { get; set; }
        public string? SocialMediaLinks { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? ProfilePictureAltText { get; set; }
        public string? ProfilePictureThumbnailUrl { get; set; }

        // --- Audit / Activity ---
        public DateTime DateJoined { get; set; } = DateTime.UtcNow;
        public DateTime LastLogin { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // --- Navigation Properties ---

        // Appointments booked by this user
        public ICollection<Appointment>? Appointments { get; set; }

        // Appointments created / updated by this user (admin or staff actions)
        [InverseProperty(nameof(Appointment.CreatedByUser))]
        public ICollection<Appointment>? CreatedAppointments { get; set; }

        [InverseProperty(nameof(Appointment.UpdatedByUser))]
        public ICollection<Appointment>? UpdatedAppointments { get; set; }

        [InverseProperty(nameof(Appointment.LastUpdatedByUser))]
        public ICollection<Appointment>? LastUpdatedAppointments { get; set; }

        // --- Constructors ---

        // Main constructor enforcing required fields
        public ApplicationUser(string fullName, string gender, string address)
        {
            FullName = fullName;
            Gender = gender;
            Address = address;
        }

        // Parameterless constructor (required by EF Core and Identity)
        public ApplicationUser()
        {
            FullName = null!;
            Gender = null!;
            Address = null!;
        }
    }
}
