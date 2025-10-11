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
        public DateTimeOffset? CreatedDate { get; set; }
        public string? BrandColor { get; set; }
        public string? BrandImageUrl { get; set; }
        public string? BrandLogo { get; set; }

        public string? PrimaryEmail { get; set; }
        public string? MatrimonyEmail { get; set; }
        public string? EventEmail { get; set; }
        public string? OrgHeadEmail { get; set; }

        public virtual ICollection<OrganizationChapter> OrganizationChapters { get; set; }
        public virtual ICollection<UserDetail> UserDetails { get; set; }
        public virtual ICollection<MatrimonialData> MatrimonialData { get; set; }
        public virtual ICollection<EventData> EventData { get; set; }
        public virtual ICollection<UserOrganization> UserOrganizations { get; set; } // Junction table collection
        public virtual ICollection<UserRole> UserRoles { get; set; } // Junction table collection
        public virtual ICollection<OrganizationConfiguration> OrganizationConfigurations { get; set; } = new List<OrganizationConfiguration>();

    }
}
