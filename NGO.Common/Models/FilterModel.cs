namespace NGO.Common.Models
{
    public class FilterModel
    {
        public int PageNumber { get; set; }

        public string? SortMember { get; set; } = null;

        public int PageSize { get; set; }

        public bool SortDescending { get; set; }

        public List<FilterOption> Filters { get; set; } = new List<FilterOption>();

        public bool IsInActive { get; set; }
    }

    public class FilterOption
    {
        public string Value { get; set; } = null!;

        public string Property { get; set; } = null!;

        public string Operator { get; set; } = null!;
    }
}
