using System;
using System.Collections.Generic;

namespace NGO.Data
{
    public partial class Organization
    {
        public Organization()
        {
            OrganizationChapters = new HashSet<OrganizationChapter>();
        }

        public int Id { get; set; }
        public string OrgName { get; set; } = null!;
        public string? OrgWelMsg { get; set; }
        public string? PayPalKey { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<OrganizationChapter> OrganizationChapters { get; set; }
        public virtual ICollection<UserDetail> UserDetails { get; set; }

    }
}
