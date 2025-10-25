using System;
using System.ComponentModel.DataAnnotations;

namespace LeRayBookingSystem.Models
{
    public class AdminNotification
    {

        public int Id { get; set; }

        // The actual message/description of the notification
        public string Message { get; set; } = null!;

        // Role(s) that should see this notification (e.g., Admin, SuperAdmin)
        public string RecipientRole { get; set; } = null!;

        // Optional link (e.g., to appointment details)
        public string? Link { get; set; }

        // Optional related entity info (e.g., "Appointment", "User")
        public string? RelatedEntity { get; set; }

        // Optional related entity ID
        public int? RelatedEntityId { get; set; }

        // Track read/unread status
        public bool IsRead { get; set; } = false;

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
