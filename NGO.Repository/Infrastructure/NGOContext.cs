﻿using System;
using Aykan.SRM.Model;
using Microsoft.EntityFrameworkCore;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                // Use PostgreSQL provider
                optionsBuilder.UseNpgsql("Host=TTTTTTep-bitter-block-a8b9ny27-pooler.eastus2.azure.neon.tech;Port=5432;Username=TTTTTTTNGO_owner;Password=TTTTTTpe03oSuDQNrf;Database=Community;Ssl Mode=Require;Trust Server Certificate=true;Pooling=true;MaxPoolSize=100;", npgsqlOptions =>
                {
                    npgsqlOptions.CommandTimeout(60); // Set command timeout to 60 seconds
                }

                );
                

                //optionsBuilder.UseNpgsql("postgresql://NGO_owner:pe03oSuDQNrf@ep-bitter-block-a8b9ny27-pooler.eastus2.azure.neon.tech/NGO?sslmode=require");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChildrensDetail>(entity =>
            {
                entity.Property(e => e.ChildUniqueId).IsUnicode(false);

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("Dob");

                entity.Property(e => e.EmailId).HasMaxLength(50);

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
                    .OnDelete(DeleteBehavior.ClientSetNull);
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
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("Organization");

                entity.Property(e => e.CreatedDate).HasColumnType("timestamptz");

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
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.Property(e => e.PayPalKey).IsUnicode(false);

                entity.Property(e => e.PaymentDate).HasColumnType("timestamptz");

                entity.Property(e => e.PaymentId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.UserDetail)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.UserDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Name).IsUnicode(false);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("timestamptz");

                entity.Property(e => e.UpdatedOn).HasColumnType("timestamptz");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.ContactNumber)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn).HasColumnType("timestamptz");

                entity.Property(e => e.Email).HasMaxLength(150);

                entity.Property(e => e.LastLogin).HasColumnType("timestamptz");

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.PaymentInfo).IsUnicode(false);

                entity.Property(e => e.UpdatedOn).HasColumnType("timestamptz");
            });

            modelBuilder.Entity<UserDetail>(entity =>
            {
                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.Country).HasMaxLength(50);

                entity.Property(e => e.CreatedOn).HasColumnType("timestamptz");

                entity.Property(e => e.Dob)
                    .HasColumnType("timestamptz")
                    .HasColumnName("Dob");

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedOn).HasColumnType("timestamptz");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserDetails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);


                entity.HasOne(d => d.Organization)
               .WithMany(p => p.UserDetails)  
               .HasForeignKey(d => d.OrgId)
               .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
