using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models;

public partial class ProjectManagementContext : DbContext
{
    public ProjectManagementContext()
    {
    }

    public ProjectManagementContext(DbContextOptions<ProjectManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MstStatus> MstStatuses { get; set; }

    public virtual DbSet<PrjIssue> PrjIssues { get; set; }

    public virtual DbSet<PrjIssueComment> PrjIssueComments { get; set; }

    public virtual DbSet<PrjProject> PrjProjects { get; set; }

    public virtual DbSet<PrjProjectMember> PrjProjectMembers { get; set; }

    public virtual DbSet<SecRole> SecRoles { get; set; }

    public virtual DbSet<SecUser> SecUsers { get; set; }

    public virtual DbSet<SprSprint> SprSprints { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MstStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__MST_Stat__C8EE2043BF6E096A");

            entity.ToTable("MST_Status");

            entity.HasIndex(e => e.StatusName, "UQ__MST_Stat__05E7698AF1D6AF1C").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PrjIssue>(entity =>
        {
            entity.HasKey(e => e.IssueId).HasName("PK__PRJ_Issu__6C8616241BC49C03");

            entity.ToTable("PRJ_Issues");

            entity.Property(e => e.IssueId).HasColumnName("IssueID");
            entity.Property(e => e.Attachment1)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Attachment2)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.RaisedOn).HasColumnType("datetime");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.PrjIssueAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__PRJ_Issue__Assig__7E37BEF6");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PrjIssueCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PRJ_Issue__Creat__7D439ABD");

            entity.HasOne(d => d.Project).WithMany(p => p.PrjIssues)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PRJ_Issue__Proje__7B5B524B");

            entity.HasOne(d => d.Status).WithMany(p => p.PrjIssues)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PRJ_Issue__Statu__7C4F7684");
        });

        modelBuilder.Entity<PrjIssueComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__PRJ_Issu__C3B4DFAAA8AC2784");

            entity.ToTable("PRJ_IssueComments");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.CommentText).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IssueId).HasColumnName("IssueID");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PrjIssueComments)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PRJ_Issue__Creat__02FC7413");

            entity.HasOne(d => d.Issue).WithMany(p => p.PrjIssueComments)
                .HasForeignKey(d => d.IssueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PRJ_Issue__Issue__02084FDA");
        });

        modelBuilder.Entity<PrjProject>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__PRJ_Proj__761ABED0E7BF8664");

            entity.ToTable("PRJ_Project");

            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.ClientName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.ProjectDescription).HasColumnType("text");
            entity.Property(e => e.ProjectManagerName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ProjectName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ProjectStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Active");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Visibility)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Private");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PrjProjects)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PRJ_Proje__Creat__59063A47");
        });

        modelBuilder.Entity<PrjProjectMember>(entity =>
        {
            entity.HasKey(e => e.ProjectMemberId).HasName("PK__PRJ_Proj__E4E9983C6E5841D2");

            entity.ToTable("PRJ_ProjectMembers");

            entity.Property(e => e.ProjectMemberId).HasColumnName("ProjectMemberID");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.RoleInProject)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Project).WithMany(p => p.PrjProjectMembers)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PRJ_Proje__Proje__5BE2A6F2");

            entity.HasOne(d => d.User).WithMany(p => p.PrjProjectMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PRJ_Proje__UserI__5CD6CB2B");
        });

        modelBuilder.Entity<SecRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__SEC_Role__8AFACE3AA503E725");

            entity.ToTable("SEC_Roles");

            entity.HasIndex(e => e.RoleName, "UQ__SEC_Role__8A2B61605C3FB6E5").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SecUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__SEC_User__1788CCACEBE686A7");

            entity.ToTable("SEC_Users");

            entity.HasIndex(e => e.Email, "UQ__SEC_User__A9D105343E0A60BE").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.SecUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SEC_Users__RoleI__534D60F1");
        });

        modelBuilder.Entity<SprSprint>(entity =>
        {
            entity.HasKey(e => e.SprintId).HasName("PK__SPR_Spri__29F16AE0BC2669A5");

            entity.ToTable("SPR_Sprint");

            entity.Property(e => e.SprintId).HasColumnName("SprintID");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.IsCompleted).HasDefaultValue(false);
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.SprintName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Project).WithMany(p => p.SprSprints)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SPR_Sprin__Proje__6D0D32F4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
