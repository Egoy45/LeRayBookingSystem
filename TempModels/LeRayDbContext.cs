using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace LeRayBookingSystem.TempModels;

public partial class LeRayDbContext : DbContext
{
    public LeRayDbContext()
    {
    }

    public LeRayDbContext(DbContextOptions<LeRayDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=127.0.0.1;port=3306;database=LeRayDB;user=root;charset=utf8mb4", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.32-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
