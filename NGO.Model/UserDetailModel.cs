namespace NGO.Model
{
    public class UserDetailModel
    {

        public int Id { get; set; }
        public string? UniqueId { get; set; }
        public int UserId { get; set; }
        public int OrgId { get; set; }
        public int ChapterId { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public string? FamilyName { get; set; }
        public string? IsApproved { get; set; }

        public string? PhoneNumber { get; set; } = null!;
        public string? AddInWhatsAppOrTelegram { get; set; } = null!;


        public string? PreferredBy { get; set; }
        public int? YearOfBirth { get; set; }

        public string? Photo { get; set; } = null!;
        public int? AddressId { get; set; }

        public string? SpouseFirstName { get; set; } = null!;
        public string? SpousePhoneNumber { get; set; }
        public string? SpouseLastName { get; set; } = null!;
        public int? SpouseYearOfBirth { get; set; }
        public string? SpouseEmail { get; set; } = null!;
        public string? SpouseCity { get; set; }
        public string? SpouseState { get; set; }
        public string? SpouseCountry { get; set; }
        public string? SpouseHometown { get; set; } = null!;
        public string? Reason { get; set; } = null!;
        public DateTimeOffset? MembershipValidityDate { get; set; }
        public string? MembershipStatus { get; set; }

        public int? CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int? ApprovedBy { get; set; } // New field for tracking who approved the membership


        public List<ChildrensDetailsModel> ChildrensDetails { get; set; }

    }
}
