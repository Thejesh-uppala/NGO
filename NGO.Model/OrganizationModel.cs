namespace NGO.Model
{
    public class OrganizationModel
    {
        public int Id { get; set; }
        public string OrgName { get; set; } = null!;
        public string? OrgWelMsg { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string? BrandImageUrl { get; set; }
        public string? BrandLogo { get; set; }
        public string? BrandColor { get; set; }

        public string? PrimaryEmail { get; set; }
        public string? MatrimonyEmail { get; set; }
        public string? EventEmail { get; set; }
        public string? OrgHeadEmail { get; set; }

        public List<OrganizationChapterModel> OrganizationChapters { get; set; }
        public List<UserDetailModel> userDetailModels { get; set; }

    }
}
