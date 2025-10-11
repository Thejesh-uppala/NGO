namespace NGO.Model
{
    public class UserBasicInfo
    {
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public string? Photo { get; set; }
        public string? PhoneNumber { get; set; }
        public int? YearOfBirth { get; set; }
        public string EmailAddress { get; set; } = null!;


    }
}

