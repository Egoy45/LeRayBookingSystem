using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeRayBookingSystem.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(255)]
        public string? ServiceName { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Status { get; set; } = "Pending";

        public string? PaymentReceiptPath { get; set; }

        [Required]
        [MaxLength(50)]
        public string? PaymentStatus { get; set; } = "Unpaid";

        [Required]
        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        public string? Notes { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }

        public string? PromoCode { get; set; }

        public Promos? Promo { get; set; }

        [Required]
        public string? PaymentId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string? CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public ApplicationUser? CreatedByUser { get; set; }

        [Required]
        public string? UpdatedBy { get; set; }

        [ForeignKey("UpdatedBy")]
        public ApplicationUser? UpdatedByUser { get; set; }

        [Required]
        public string? LastUpdatedBy { get; set; }

        [ForeignKey("LastUpdatedBy")]
        public ApplicationUser? LastUpdatedByUser { get; set; }

        public string? CancellationReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;

        // Parameterless constructor
        public Appointment() { }

        // Simplified constructor
        public Appointment(string userId, string serviceName, string paymentMethod)
        {
            UserId = userId;
            ServiceName = serviceName;
            PaymentMethod = paymentMethod;

            Status = "Pending";
            PaymentStatus = "Unpaid";
            PaymentId = Guid.NewGuid().ToString();
            CreatedAt = UpdatedAt = LastUpdatedAt = AppointmentDate = DateTime.Now;

            CreatedBy = UpdatedBy = LastUpdatedBy = userId;
        }

        // Full constructor
        public Appointment(
            int id,
            string userId,
            string serviceName,
            DateTime appointmentDate,
            string status,
            string? paymentReceiptPath,
            string paymentStatus,
            string? notes,
            int serviceId,
            string? promoCode,
            string paymentMethod)
        {
            Id = id;
            UserId = userId;
            ServiceName = serviceName;
            AppointmentDate = appointmentDate;
            Status = status;
            PaymentReceiptPath = paymentReceiptPath;
            PaymentStatus = paymentStatus;
            Notes = notes;
            ServiceId = serviceId;
            PromoCode = promoCode;
            PaymentMethod = paymentMethod;

            CreatedAt = UpdatedAt = LastUpdatedAt = DateTime.Now;
            PaymentId = Guid.NewGuid().ToString();
            CreatedBy = UpdatedBy = LastUpdatedBy = userId;
        }
    }
}
