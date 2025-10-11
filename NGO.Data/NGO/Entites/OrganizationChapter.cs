namespace NGO.Data
{
    public partial class OrganizationChapter
    {
        public int Id { get; set; }
        public int OrgId { get; set; }
        public string ChapterName { get; set; } = null!;
        public string ChapterPresidentName { get; set; } = null!;
        public string ChapterPresidentEmail { get; set; } = null!;
        public string ChapterPresidentPhone { get; set; }
        public string Details { get; set; } = null!;

        public virtual Organization Org { get; set; } = null!;
    }
}
