using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Models;

namespace SchoolApp.Data;

public partial class SchoolMvcDbfirstContext : DbContext
{
    public SchoolMvcDbfirstContext()
    {
    }

    public SchoolMvcDbfirstContext(DbContextOptions<SchoolMvcDbfirstContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Capability> Capabilities { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DevConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Greek_100_CI_AI");

        modelBuilder.Entity<Capability>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Capabilities_Name").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);  // Default
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasIndex(e => e.Description, "IX_Courses_Description");

            entity.HasIndex(e => e.TeacherId, "IX_Courses_TeacherId");

            entity.Property(e => e.Description).HasMaxLength(50);

            entity.HasOne(d => d.Teacher).WithMany(p => p.Courses)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_Courses_Teachers");

            entity.HasMany(d => d.Students).WithMany(p => p.Courses)
                .UsingEntity<Dictionary<string, object>>(
                    "CoursesStudent",
                    r => r.HasOne<Student>().WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CoursesStudents_Students"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CoursesStudents_Courses"),
                    j =>
                    {
                        j.HasKey("CourseId", "StudentId");
                        j.ToTable("CoursesStudents");
                        j.HasIndex(new[] { "CourseId" }, "IX_CoursesStudents_CourseId");
                        j.HasIndex(new[] { "StudentId" }, "IX_CoursesStudents_StudentId");
                    });
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Roles_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasMany(d => d.Capabilities).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolesCapability",
                    r => r.HasOne<Capability>().WithMany()
                        .HasForeignKey("CapabilityId")
                        .HasConstraintName("FK_RolesCapabilities_Capabilities"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_RolesCapabilities_Roles"),
                    j =>
                    {
                        j.HasKey("RoleId", "CapabilityId");
                        j.ToTable("RolesCapabilities");
                        j.HasIndex(new[] { "CapabilityId" }, "IX_RolesCapabilities_CapabilityId");
                    });
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(e => e.Am, "IX_Students_AM").IsUnique();

            entity.HasIndex(e => e.Institution, "IX_Students_Institution");

            entity.HasIndex(e => e.UserId, "IX_Students_UserId").IsUnique();

            entity.Property(e => e.Am).HasMaxLength(10);
            entity.Property(e => e.Department).HasMaxLength(50);
            entity.Property(e => e.Institution).HasMaxLength(50);

            entity.HasOne(d => d.User).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_Users");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasIndex(e => e.Institution, "IX_Teachers_Institution");

            entity.HasIndex(e => e.UserId, "IX_Teachers_UserId").IsUnique();

            entity.Property(e => e.Institution).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.User).WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Teachers_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();

            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");

            entity.HasIndex(e => e.Username, "IX_Users_Username").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Firstname).HasMaxLength(50);
            entity.Property(e => e.Lastname).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(60);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
