
using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;
using NGO.Data;
using NGO.Data.NGO.Entites;

namespace NGO.Repository
{
    public partial class NGOContext : DbContext, IDataProtectionKeyContext
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public NGOContext()
        {
        }


        public NGOContext(DbContextOptions<NGOContext> options, IDataProtectionProvider dataProtectionProvider)
            : base(options)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public virtual DbSet<MatrimonialData> MatrimonialData { get; set; } = null!;
        public virtual DbSet<MatrimonialDataPhoto> MatrimonialDataPhotos { get; set; } = null!;
        public virtual DbSet<OrganizationConfiguration> OrganizationConfigurations { get; set; } = null!;
        public virtual DbSet<MembershipData> MembershipData { get; set; } = null!;
        public virtual DbSet<EventPayment> EventPayments { get; set; } = null!;
        public virtual DbSet<Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey> DataProtectionKeys { get; set; } = null!;
        public virtual DbSet<ChildrensDetail> ChildrensDetails { get; set; } = null!;
        public virtual DbSet<MemberShipType> MembershipUserTypes { get; set; } = null!;
        public virtual DbSet<Organization> Organizations { get; set; } = null!;
        public virtual DbSet<OrganizationChapter> OrganizationChapters { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserDetail> UserDetails { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;
        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<EventData> EventData { get; set; } = null!;
        public virtual DbSet<EventCost> EventCosts { get; set; } = null!;
        public virtual DbSet<ParticipantType> ParticipantTypes { get; set; } = null!;
        public virtual DbSet<UserEventRegistration> UserEventRegistration { get; set; } = null!;
        public virtual DbSet<GuestEventRegistration> GuestUsersEventRegistration { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = Environment.GetEnvironmentVariable("AppSettings__ConnectionString")
            ?? throw new InvalidOperationException("The 'AppSettings__ConnectionString' environment variable is missing or null.");

                optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.CommandTimeout(60); // Set command timeout to 60 seconds
                    npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5, // Number of retry attempts
                    maxRetryDelay: TimeSpan.FromSeconds(5), // Delay between retries
                    errorCodesToAdd: null // Default SQL Server transient error codes
                );
                });

            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔹 Global PostgreSQL conventions
            modelBuilder.HasPostgresExtension("uuid-ossp"); // For uuid_generate_v4()
            modelBuilder.UseSerialColumns(); // Aligns with IDENTITY columns


            modelBuilder.Entity<UserEventRegistration>()
               .HasOne(m => m.EventData)
               .WithMany(e => e.UserEventRegistration)
               .HasForeignKey(m => m.EventId)
                 .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<UserEventRegistration>()
                .HasOne(m => m.User)
                .WithMany(u => u.UserEventRegistration)
                .HasForeignKey(m => m.UserId);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GuestEventRegistration>(entity =>
            {
                entity.Property(e => e.Cost)
                    .HasMaxLength(10)
                    .IsRequired();

                entity.HasOne(e => e.EventData)
                    .WithMany(e => e.GuestUsersEventRegistration)
                    .HasForeignKey(e => e.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.GuestUsersEventRegistration)
                    .HasForeignKey(e => e.AssociatedWithUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<EventPayment>(entity =>
            {
                entity.ToTable("EventPayment");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(100)
                    .IsRequired();



                entity.Property(e => e.CheckNumber)
                    .HasMaxLength(50);

                entity.Property(e => e.BankName)
                    .HasMaxLength(100);

                entity.Property(e => e.TotalCost)
                    .HasColumnType("decima(10,2)")
                    .IsRequired();

                entity.HasOne(e => e.UserDetail)
                    .WithMany(u => u.EventPayments)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.EventData)
                    .WithMany(e => e.EventPayments)
                    .HasForeignKey(e => e.EventId)
                    .OnDelete(DeleteBehavior.Cascade);
            });



            // EventData Configuration
            modelBuilder.Entity<EventData>(entity =>
            {
                entity.ToTable("EventData");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.EventTitle).HasMaxLength(1000);
                entity.Property(e => e.EventDesc).IsRequired();
                entity.Property(e => e.EventRequirements).HasMaxLength(1000);
                entity.Property(e => e.EventContactEmail).HasMaxLength(100);
                entity.Property(e => e.EventContactPhone).HasMaxLength(50);
                entity.Property(e => e.EventAccess).HasMaxLength(50);
                entity.Property(e => e.EventTimeUtc).HasMaxLength(500);
                entity.Property(e => e.EventLocation).HasMaxLength(500);

                entity.HasMany(e => e.EventCosts)
                      .WithOne(c => c.EventData)
                      .HasForeignKey(c => c.EventId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Organization)
                .WithMany(p => p.EventData)
                .HasForeignKey(d => d.OrgId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<EventCost>(entity =>
            {
                entity.ToTable("EventCosts");
                entity.HasKey(e => e.Id);

                entity.HasOne(ec => ec.EventData)
                      .WithMany(e => e.EventCosts)
                      .HasForeignKey(ec => ec.EventId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ec => ec.ParticipantType)
                      .WithMany(pt => pt.EventCosts)
                      .HasForeignKey(ec => ec.ParticipantTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<ParticipantType>(entity =>
            {
                entity.ToTable("ParticipantTypes");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasIndex(e => e.Name)
                      .IsUnique(); // Matches the UNIQUE constraint in SQL
            });

            modelBuilder.Entity<MatrimonialData>(entity =>
            {
                entity.ToTable("MatrimonialData");

                entity.HasOne(m => m.Approver)
    .WithMany()
    .HasForeignKey(m => m.ApprovedBy)
    .OnDelete(DeleteBehavior.Restrict)
       .IsRequired(false);

                entity.HasOne(e => e.user)               // MatrimonialData has one User
                      .WithMany(u => u.MatrimonialData)   // User has many MatrimonialData
                      .HasForeignKey(e => e.AssociatedWithUserId)        // Foreign key is UserId in MatrimonialData
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Organization)
     .WithMany(p => p.MatrimonialData)
     .HasForeignKey(d => d.OrgId)
     .OnDelete(DeleteBehavior.ClientSetNull);

            });

            modelBuilder.Entity<MatrimonialDataPhoto>(entity =>
            {
                modelBuilder.Entity<MatrimonialDataPhoto>()
                 .Property(m => m.Id)
                 .ValueGeneratedOnAdd();

                entity.HasOne(e => e.MatrimonialData)
                    .WithMany(d => d.MatrimonialDataPhotos)
                    .HasForeignKey(e => e.MatrimonailId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });


            modelBuilder.Entity<ChildrensDetail>(entity =>
            {
                entity.Property(e => e.ChildUniqueId).IsUnicode(false);



                entity.Property(e => e.EmailId).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.PhoneNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ResidentCity).HasMaxLength(100);

                entity.Property(e => e.ResidentCountry).HasMaxLength(100);

                entity.Property(e => e.ResidentState).HasMaxLength(100);

                entity.HasOne(d => d.UserDetail)
     .WithMany(p => p.ChildrensDetails)
     .HasForeignKey(d => d.UserDetailId)
     .HasPrincipalKey(p => p.UserId)  // This specifies UserId as the referenced key
     .OnDelete(DeleteBehavior.ClientSetNull);


            });




            modelBuilder.Entity<MemberShipType>(entity =>
            {
                entity.HasKey(e => new { e.UserDetailId, e.MembershipId });


                entity.HasOne(d => d.UserDetail)
           .WithMany(p => p.MembershipUserTypes)
           .HasForeignKey(d => d.UserDetailId)
           .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.MembershipData)
                            .WithMany(p => p.MembershipUserTypes)
                            .HasForeignKey(d => d.MembershipId)
                            .OnDelete(DeleteBehavior.ClientSetNull);

            });

            modelBuilder.Entity<MembershipData>(entity =>
            {
                entity.ToTable("MembershipData");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.MemberShipType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MemberShipAmount)
                    .HasColumnType("bigint");

                entity.Property(e => e.ValidityPeriod)
                    .HasColumnType("int");

                entity.HasOne(e => e.Organization)
        .WithMany()  // Assuming Organization has no navigation property back to MembershipData
        .HasForeignKey(e => e.OrgId)
        .HasConstraintName("FK_MembershipData_Organization");
            });


            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("Organization");

                entity.Property(e => e.CreatedDate).HasColumnType("timestamp with time zone");

                entity.Property(e => e.OrgName)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.OrgWelMsg).IsUnicode(false);


                entity.Property(e => e.BrandColor)
            .HasMaxLength(20)  // assuming hex color code format (e.g., #FFFFFF)
            .IsUnicode(false);

                entity.Property(e => e.BrandImageUrl)
                    .HasMaxLength(500)  // length for URL or file path
                    .IsUnicode(false);


            });

            modelBuilder.Entity<UserOrganization>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.OrganizationId });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserOrganizations)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);


                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.UserOrganizations)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
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



                entity.Property(e => e.PaymentDate).HasColumnType("timestamp with time zone");



                entity.HasOne(d => d.UserDetail)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.UserDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Name).IsUnicode(false);
            });



            modelBuilder.Entity<Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey>(entity =>
            {
                entity.ToTable("DataProtectionKeys");
                entity.HasKey(e => e.Id); // Explicit primary key
                entity.Property(e => e.Id)
                      .ValueGeneratedOnAdd(); // Ensure Id is auto-incremented
                entity.Property(e => e.FriendlyName).HasMaxLength(255);
                entity.Property(e => e.Xml).IsRequired();
                // No navigation properties or relationships defined
            });


            modelBuilder.Entity<User>(entity =>
            {


                entity.Property(e => e.CreatedOn).HasColumnType("timestamp with time zone");

                entity.Property(e => e.Email).HasMaxLength(150);

                entity.Property(e => e.LastLogin).HasColumnType("timestamp with time zone");

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);


                entity.Property(e => e.UpdatedOn).HasColumnType("timestamp with time zone");

                entity.Property(e => e.Status).IsRequired(); // Add Status configuration
                entity.Property(e => e.CreatedBy).IsRequired(); // Add CreatedBy configuration
                entity.Property(e => e.UpdatedBy).IsRequired(); // Add UpdatedBy configuration
                entity.Property(e => e.IsDeleted).IsRequired(); // Add IsDeleted configuration
                entity.Property(e => e.OTP)
          .HasMaxLength(6) // Adjust as per OTP length requirement
          .IsUnicode(false);

                entity.Property(e => e.OTPUsed)
                    .HasDefaultValue(false);


            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.State).HasMaxLength(100);
                entity.Property(e => e.PostalCode).HasMaxLength(100);
                entity.Property(e => e.Country).HasMaxLength(100);
                entity.Property(e => e.HomeTown).HasMaxLength(100);
            });




            modelBuilder.Entity<UserDetail>(entity =>
            {


                entity.Property(e => e.MembershipValidityDate)
          .HasColumnType("timestamp with time zone")
          .HasColumnName("MembershipValidityDate");

                entity.Property(e => e.MembershipStatus)
                      .HasMaxLength(50)
                      .HasColumnName("MembershipStatus");

                modelBuilder.Entity<UserDetail>()
               .HasOne(u => u.AddressEntity)
               .WithMany(a => a.UserDetails)
               .HasForeignKey(u => u.AddressId);

                modelBuilder.Entity<UserDetail>()
.HasOne(u => u.Approver)
.WithMany()
.HasForeignKey(u => u.ApprovedBy)
.OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.CreatedOn).HasColumnType("timestamp with time zone");



                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedOn).HasColumnType("timestamp with time zone");
                entity.Property(e => e.Photo).HasMaxLength(500);  // You can adjust the max length as needed

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

                entity.Property(ur => ur.Id)
                    .ValueGeneratedOnAdd();

                entity.HasKey(e => new { e.UserId, e.OrgId, e.RoleId }); // Adjusted to include OrgId in the key
                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.Organization) // Ensure you have this navigation property defined in UserRole
                    .WithMany(p => p.UserRoles) // Ensure the reverse navigation property in Organization exists
                    .HasForeignKey(d => d.OrgId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OrganizationConfiguration>(entity =>
            {
                entity.ToTable("OrganizationConfiguration");

                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.OrgId, "idx_orgconfig_orgid");
                //entity.Property(e => e.Secret).IsRequired();
                entity.Property(e => e.SessionExpiryInMinutes).HasDefaultValue(60);
                entity.Property(e => e.ShortExpiryInMinutes).HasDefaultValue(1440);
                entity.Property(e => e.LongExpiryInMinutes).HasDefaultValue(525600);
                entity.Property(e => e.MaxRetryCount).HasDefaultValue(5);
                entity.Property(e => e.EmailHost).HasMaxLength(255).HasDefaultValue("smtp.gmail.com");
                entity.Property(e => e.EmailPort).HasDefaultValue(587);
                entity.Property(e => e.EnableSSL).HasDefaultValue(true);
                entity.Property(e => e.EmailDefaultId).HasMaxLength(255).IsRequired();
                entity.Property(e => e.EmailPassword).IsRequired();
                entity.Property(e => e.EmailTimeOut).HasDefaultValue(20000);
                entity.Property(e => e.CloudinaryCloudName).IsRequired();
                entity.Property(e => e.CloudinaryApiKey).IsRequired();
                entity.Property(e => e.CloudinaryApiSecret).IsRequired();

                //for SQL
                //entity.Property(e => e.CreatedDate).HasColumnType("datetimeoffset").HasDefaultValueSql("GETDATE()");
                //entity.Property(e => e.UpdatedDate).HasColumnType("datetimeoffset");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedDate)
                      .HasColumnType("timestamp with time zone");

                entity.HasOne(e => e.Organization)
                      .WithMany(o => o.OrganizationConfigurations)
                      .HasForeignKey(e => e.OrgId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 🔹 Update all timestamp with time zone columns globally
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType == typeof(DateTimeOffset))
                    {
                        property.SetColumnType("timestamp with time zone");
                    }
                }
            }



            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}