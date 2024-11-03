using System;
using System.Collections.Generic;
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
        public string? PaymentInfo { get; set; }
        public string Name { get; set; } = null!;
        public string ContactNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Status { get; set; }
        public int? UnsuccessfulLoginAttempts { get; set; }
        public DateTime? LastLogin { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }= DateTime.UtcNow;
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }= DateTime.UtcNow;
        public bool IsDeleted { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<UserOrganization> UserOrganizations { get; set; } // Junction table collection
        public virtual ICollection<UserDetail> UserDetails { get; set; }
    }
}
