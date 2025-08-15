using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeRayBookingSystem.Models
{
    public class Promos
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string? Description { get; set; }    

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        public TimeSpan Duration { get; set; }

        [Required]
        [MaxLength(50)]
        public string? PromoCode { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public string? CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public ApplicationUser? CreatedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
