using System;
using System.Collections.Generic;
using Aykan.SRM.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NGO.Data;
using NGO.Model;

namespace NGO.Repository
{
    public partial class NGOContext : DbContext
    {
        public NGOContext()
        {
        }

        public NGOContext(DbContextOptions<NGOContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ChildrensDetail> ChildrensDetails { get; set; } = null!;
        public virtual DbSet<MemberShipType> MemberShipTypes { get; set; } = null!;
        public virtual DbSet<Organization> Organizations { get; set; } = null!;
        public virtual DbSet<OrganizationChapter> OrganizationChapters { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<RolePermission> RolePermissions { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserDetail> UserDetails { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;
        public virtual DbSet<UspUserReturnModel> UspUserReturnModels { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChildrensDetail>(entity =>
            {
                entity.Property(e => e.ChildUniqueId).IsUnicode(false);

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("DOB");

                entity.Property(e => e.EmialId).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.PhoneNo)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ResidentCity).HasMaxLength(50);

                entity.Property(e => e.ResidentCountry).HasMaxLength(50);

                entity.Property(e => e.ResidentState).HasMaxLength(50);

                entity.HasOne(d => d.UserDetail)
                    .WithMany(p => p.ChildrensDetails)
                    .HasForeignKey(d => d.UserDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChildrensDetails_ChildrensDetails");
            });

            modelBuilder.Entity<MemberShipType>(entity =>
            {
                entity.Property(e => e.MemberShipType1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MemberShipType");

                entity.HasOne(d => d.UserDetail)
                    .WithMany(p => p.MemberShipTypes)
                    .HasForeignKey(d => d.UserDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MemberShipTypes_UserDetails");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("Organization");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.OrgName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OrgWelMsg).IsUnicode(false);

                entity.Property(e => e.PayPalKey).IsUnicode(false);
            });

            modelBuilder.Entity<OrganizationChapter>(entity =>
            {
                entity.ToTable("OrganizationChapter");

                entity.Property(e => e.ChapterName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ChapterPresidentEmail)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ChapterPresidentName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Details).IsUnicode(false);

                entity.HasOne(d => d.Org)
                    .WithMany(p => p.OrganizationChapters)
                    .HasForeignKey(d => d.OrgId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganizationChapter_OrganizationChapter");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.Property(e => e.PayPalKey).IsUnicode(false);

                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.UserDetail)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.UserDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payment_Payment");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Name).IsUnicode(false);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePermissions_Roles");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.ContactNumber)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(150);

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.PaymentInfo).IsUnicode(false);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserDetail>(entity =>
            {
                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.ChapterId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.Country).HasMaxLength(50);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Dob)
                    .HasColumnType("datetime")
                    .HasColumnName("DOB");

                entity.Property(e => e.FamilyName).IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.HomeTown).HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OrgId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PreferredBy).HasMaxLength(250);

                entity.Property(e => e.SocialMedia).HasMaxLength(15);

                entity.Property(e => e.SpouseCity)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SpouseCountry)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SpouseDob)
                    .HasColumnType("datetime")
                    .HasColumnName("SpouseDOB");

                entity.Property(e => e.SpouseEmail).IsUnicode(false);

                entity.Property(e => e.SpouseFirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SpouseHometown)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SpouseLastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SpousePhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SpouseState)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.State)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UniqueId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("uniqueId");

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.Property(e => e.WhatsAppNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserDetails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserDetails_Users");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Users");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
