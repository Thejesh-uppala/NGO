namespace NGO.Model
{
    public class FileModel
    {
        public FileModel()
        {
            SpouseDetails = new SpouseDetailsData();
            ChildrensDetails = new List<ChildrensDetailsDataModel>();
        }

        public string FirstName { get; set; } = null!;
        public int OrgId { get; set; }
        public string? MemberId { get; set; } = null!;


        public string? FamilyName { get; set; } = null!;
        public string? IsApproved { get; set; }

        public string EmailAddress { get; set; } = null!;
        public int? YearOfBirth { get; set; }
        public string? LastName { get; set; } = null!;
        public string? PostalCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Country { get; set; } = null!;
        public int UserId { get; set; }
        public string? PreferredBy { get; set; } = null!;
        public string? Reason { get; set; } = null!;
        public string? HomeTown { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? AddInWhatsAppOrTelegram { get; set; }
        public string? Photo { get; set; } = null!;



        public DateTimeOffset? MembershipValidityDate { get; set; }
        public string? MembershipStatus { get; set; }
        public int? SpouseYearOfBirth { get; set; }
        public string? ApprovedBy { get; set; }


        public string PaymentMethod { get; set; } = null!; // Payment method (Zelle, PayPal, Check)
        public string? TransactionId { get; set; } // Transaction ID for Zelle/PayPal
        public DateTimeOffset? PaymentDate { get; set; } // Payment date for Zelle/PayPal
        public string? CheckNumber { get; set; } // Check number for Check payments
        public string? BankName { get; set; } // Bank name for Check payments
        public DateTimeOffset? CheckDate { get; set; } // Check date for Check payments
        public decimal Amount { get; set; } // Payment amount


        public List<ChildrensDetailsDataModel>? ChildrensDetails { get; set; }
        public SpouseDetailsData? SpouseDetails { get; set; }


        public class ChildrensDetailsDataModel
        {
            public string? ChildCountry { get; set; } = null!;
            public string? ChildUniqueId { get; set; } = null!;
            public string? ChildCity { get; set; } = null!;
            public string? ChildState { get; set; } = null!;
            public string? ChildPhoneNumber { get; set; } = null!;
            public int? ChildYearOfBirth { get; set; }
            public string? ChildLastName { get; set; }
            public string? ChildEmailAddress { get; set; } = null!;
            public string ChildFirstName { get; set; } = null!;
        }
        public class SpouseDetailsData
        {
            public string SpouseEmail { get; set; } = null!;
            public string? SpouseCity { get; set; }
            public string? SpouseState { get; set; }
            public string? SpouseCountry { get; set; }
            public string? SpouseHometown { get; set; } = null!;
            public string SpouseFirstName { get; set; } = null!;
            public string? SpouseLastName { get; set; } = null!;
            public string? SpousePhoneNumber { get; set; } = null!;
            public int? SpouseYearOfBirth { get; set; }
        }
    }
}
