using LeRayBookingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LeRayBookingSystem.ViewModels;

namespace LeRayBookingSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Promos> Promos { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
                entity.Property(e => e.PhoneNumber).HasColumnType("varchar(20)");
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
        public DbSet<LeRayBookingSystem.ViewModels.ApplicationUserViewModel> ApplicationUserViewModel { get; set; } = default!;
    }
}
