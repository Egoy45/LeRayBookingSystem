﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeRayBookingSystem.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        // --- Relationship to ApplicationUser (the client who booked) ---
        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }

        // --- Appointment Details ---
        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required, MaxLength(255)]
        public string ServiceName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public string? Notes { get; set; }

        // --- Service Relationship ---
        [Required]
        public int ServiceId { get; set; }

        [ForeignKey(nameof(ServiceId))]
        public Service? Service { get; set; }

        // --- Promo Relationship (Optional) ---
        public int? PromoId { get; set; }

        [ForeignKey(nameof(PromoId))]
        public Promos? Promo { get; set; }

        [MaxLength(50)]
        public string? PromoCode { get; set; }

        // --- Payment Details ---
        [Required, MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string PaymentStatus { get; set; } = "Unpaid";

        public string? PaymentReceiptPath { get; set; }

        [Required]
        public string PaymentId { get; set; } = Guid.NewGuid().ToString();

        // --- Audit Trail (Created / Updated By) ---
        [Required]
        [ForeignKey(nameof(CreatedByUser))]
        public string CreatedBy { get; set; } = string.Empty;

        public ApplicationUser? CreatedByUser { get; set; }

        [Required]
        [ForeignKey(nameof(UpdatedByUser))]
        public string UpdatedBy { get; set; } = string.Empty;

        public ApplicationUser? UpdatedByUser { get; set; }

        [Required]
        [ForeignKey(nameof(LastUpdatedByUser))]
        public string LastUpdatedBy { get; set; } = string.Empty;

        public ApplicationUser? LastUpdatedByUser { get; set; }

        // --- Cancellation Info ---
        public string? CancellationReason { get; set; }

        // --- Timestamps ---
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;

        // --- Constructors ---

        public Appointment() { }

        public Appointment(string userId, string serviceName, string paymentMethod)
        {
            UserId = userId;
            ServiceName = serviceName;
            PaymentMethod = paymentMethod;

            Status = "Pending";
            PaymentStatus = "Unpaid";
            PaymentId = Guid.NewGuid().ToString();

            AppointmentDate = DateTime.Now;
            CreatedAt = UpdatedAt = LastUpdatedAt = DateTime.Now;

            CreatedBy = UpdatedBy = LastUpdatedBy = userId;
        }

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
            int? promoId,
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
            PromoId = promoId;
            PromoCode = promoCode;
            PaymentMethod = paymentMethod;

            PaymentId = Guid.NewGuid().ToString();
            CreatedBy = UpdatedBy = LastUpdatedBy = userId;
            CreatedAt = UpdatedAt = LastUpdatedAt = DateTime.Now;
        }
    }
}
