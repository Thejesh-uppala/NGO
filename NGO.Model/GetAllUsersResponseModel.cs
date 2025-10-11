namespace NGO.Model
{

    public class GetAllUsersResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public int? YearOfBirth { get; set; }
        public string? FamilyName { get; set; } = null!;
        public string? IsApproved { get; set; }

        public string? PostalCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string? PreferredBy { get; set; } = null!;
        public string? HomeTown { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Photo { get; set; } = null!;
        public string? MemberId { get; set; } = null!;
        public DateTimeOffset? MembershipValidityDate { get; set; }
        public string? MembershipStatus { get; set; }

    }

}

