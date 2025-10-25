using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeRayBookingSystem.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        // ✅ Titles and Descriptions
        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string ServiceName { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        // ✅ Pricing and Duration
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public TimeSpan Duration { get; set; } = new TimeSpan(1, 0, 0);

        // ✅ Categorization and Status
        [Required]
        public string Category { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // ✅ Public Reference Code (unique code for clients)
       // [Required]
      //  public string PublicServiceCode { get; set; } = string.Empty;

        // ✅ Image
        public string? ImagePath { get; set; }

        // ✅ Audit Fields
        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        [ForeignKey("CreatedBy")]
        public ApplicationUser? CreatedByUser { get; set; }

        [Required]
        public string UpdatedBy { get; set; } = string.Empty;

        [ForeignKey("UpdatedBy")]
        public ApplicationUser? UpdatedByUser { get; set; }

        [Required]
        public string LastUpdatedBy { get; set; } = string.Empty;

        [ForeignKey("LastUpdatedBy")]
        public ApplicationUser? LastUpdatedByUser { get; set; }

        // ✅ Auto timestamps
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;
    }
}
