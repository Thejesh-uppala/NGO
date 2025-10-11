namespace NGO.Model
{
    public class ChildrensDetailsModel
    {
        public int Id { get; set; }
        public int UserDetailId { get; set; }
        public string? ChildUniqueId { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public string? EmailId { get; set; } = null!;
        public string? PhoneNo { get; set; } = null!;
        public int? YearOfBirth { get; set; }
        public string? ResidentCity { get; set; } = null!;
        public string? ResidentState { get; set; } = null!;
        public string? ResidentCountry { get; set; } = null!;
    }
}
