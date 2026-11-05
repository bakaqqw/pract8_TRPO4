using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace pract8_TRPO4.Models;

public partial class UdilovTrpo48zadContext : DbContext
{
    public UdilovTrpo48zadContext()
    {
    }

    public UdilovTrpo48zadContext(DbContextOptions<UdilovTrpo48zadContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=udilov_trpo4_8zad;TrustServerCertificate=True;");

    //"Server=sql.ects;Database=udilov_trpo4_8zad;User Id=student_00;Password=student_00;TrustServerCertificate=True;"

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>(entity =>
        {
            entity.ToTable("Task");

            entity.Property(e => e.Id)
            .HasDefaultValueSql("NEXT VALUE FOR dbo.TaskIdSequence")
            .HasColumnName("ID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id)
            .HasDefaultValueSql("NEXT VALUE FOR dbo.UserIdSequence")
            .HasColumnName("ID");
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
