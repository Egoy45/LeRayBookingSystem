using LeRayBookingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LeRayBookingSystem.ViewModels;
using System.Diagnostics.CodeAnalysis; // Added for Code Analysis Attributes

namespace LeRayBookingSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // --- Model DbSets (These map to database tables) ---
        public DbSet<Appointment> Appointments { get; set; } = default!; // Added default!
        public DbSet<Service> Services { get; set; } = default!;       // Added default!
        public DbSet<Feedback> Feedbacks { get; set; } = default!;     // Added default!
        public DbSet<Promos> Promos { get; set; } = default!;         // Added default!
        public DbSet<Payments> Payments { get; set; } = default!;       // Added default!
        public DbSet<AuditLog> AuditLogs { get; set; } = default!;     // Added default!
        
        // --- REMOVED: DbSet<ApplicationUserViewModel> ---
        // ViewModels should NEVER be added as a DbSet, as they do not map to a database table.

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // --- Identity/MySQL Compatibility Fixes and Cleanup ---

            // Fix for Appointment foreign key (Assuming ApplicationUser.Id is string)
            builder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                // Use DeleteBehavior.ClientSetNull to prevent cascade delete issues
                .OnDelete(DeleteBehavior.ClientSetNull); 

            // Standard Identity Entity Configuration (Your existing type mappings for MySQL/MariaDB)

            // IdentityRole
            builder.Entity<IdentityRole>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("varchar(255)");
                entity.Property(e => e.Name).HasMaxLength(256).HasColumnType("varchar(256)");
                entity.Property(e => e.NormalizedName).HasMaxLength(256).HasColumnType("varchar(256)");
                entity.Property(e => e.ConcurrencyStamp).HasColumnType("longtext");
            });

            // ApplicationUser (inherits IdentityUser)
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("varchar(255)");
                entity.Property(e => e.UserName).HasMaxLength(256).HasColumnType("varchar(256)");
                entity.Property(e => e.NormalizedUserName).HasMaxLength(256).HasColumnType("varchar(256)");
                entity.Property(e => e.Email).HasMaxLength(256).HasColumnType("varchar(256)");
                entity.Property(e => e.NormalizedEmail).HasMaxLength(256).HasColumnType("varchar(256)");
                entity.Property(e => e.ConcurrencyStamp).HasColumnType("longtext");
                entity.Property(e => e.SecurityStamp).HasColumnType("longtext");
                // Ensures PhoneNumber has a defined type
                entity.Property(e => e.PhoneNumber).HasColumnType("varchar(20)"); 
                // Add your custom properties here if they are not already handled in ApplicationUser class
                // entity.Property(e => e.FirstName).HasColumnType("varchar(100)"); 
                // entity.Property(e => e.LastName).HasColumnType("varchar(100)"); 
            });

            // IdentityUserToken
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(e => e.Value).HasColumnType("longtext");
            });

            // IdentityUserLogin
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(e => e.ProviderDisplayName).HasColumnType("longtext");
            });

            // IdentityUserClaim
            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.Property(e => e.ClaimType).HasColumnType("longtext");
                entity.Property(e => e.ClaimValue).HasColumnType("longtext");
            });

            // IdentityRoleClaim
            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.Property(e => e.ClaimType).HasColumnType("longtext");
                entity.Property(e => e.ClaimValue).HasColumnType("longtext");
            });
        }
    }
}