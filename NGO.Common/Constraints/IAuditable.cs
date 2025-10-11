namespace NGO.Common.Constraints
{
    public interface IAuditable
    {
        int CreatedBy { get; set; }
        DateTimeOffset CreatedOn { get; set; }
        int UpdatedBy { get; set; }
        DateTimeOffset UpdatedOn { get; set; }
    }
}
