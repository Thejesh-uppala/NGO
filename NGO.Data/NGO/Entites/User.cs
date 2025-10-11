using NGO.Data.NGO.Entites;

namespace NGO.Data
{
    public partial class User
    {
        public User()
        {
            UserDetails = new HashSet<UserDetail>();
            UserRoles = new HashSet<UserRole>();
            UserOrganizations = new HashSet<UserOrganization>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ContactNumber { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Status { get; set; }
        public DateTimeOffset? LastLogin { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; } = DateTime.UtcNow;
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
        public int? OTP { get; set; }
        public bool? OTPUsed { get; set; }

        public virtual ICollection<MatrimonialData> MatrimonialData { get; set; } = new List<MatrimonialData>();

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<UserEventRegistration> UserEventRegistration { get; set; }
        public virtual ICollection<GuestEventRegistration> GuestUsersEventRegistration { get; set; }

        public virtual ICollection<UserOrganization> UserOrganizations { get; set; } // Junction table collection
        public virtual ICollection<UserDetail> UserDetails { get; set; }
    }
}
