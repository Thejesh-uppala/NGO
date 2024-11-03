using System;
using System.Collections.Generic;
using NGO.Data.NGO.Entites;

namespace NGO.Data
{
    public partial class Organization
    {
        public Organization()
        {
            OrganizationChapters = new HashSet<OrganizationChapter>();
            UserDetails = new HashSet<UserDetail>();
            UserOrganizations = new HashSet<UserOrganization>();
            UserRoles = new HashSet<UserRole>();
        }

        public int Id { get; set; }
        public string OrgName { get; set; } = null!;
        public string? OrgWelMsg { get; set; }
        public string? PayPalKey { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<OrganizationChapter> OrganizationChapters { get; set; }
        public virtual ICollection<UserDetail> UserDetails { get; set; }
        public virtual ICollection<UserOrganization> UserOrganizations { get; set; } // Junction table collection
        public virtual ICollection<UserRole> UserRoles { get; set; } // Junction table collection

    }
}
