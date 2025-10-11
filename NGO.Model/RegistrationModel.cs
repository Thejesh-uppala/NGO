namespace NGO.Model
{
    public class RegistrationModel
    {
        public RegistrationModel()
        {
            SpouseDetails = new SpouseDetailsRegisterData();
            ChildrensDetails = new List<ChildrensDetailsRegisterDataModel>();
        }

        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string? FamilyName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public int MemberShipId { get; set; }
        public int? YearOfBirth { get; set; }
        public string? LastName { get; set; } = null!;
        public string? PostalCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string? HomeTown { get; set; } = null!;
        public string? Reason { get; set; } = null!;
        public string PreferredBy { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? IsApproved { get; set; }

        public string? AddInWhatsAppOrTelegram { get; set; }

        // Payment Details
        public string PaymentMethod { get; set; } = null!; // Zelle, PayPal, Check
        public string? TransactionId { get; set; } // For Zelle/PayPal
        public DateTimeOffset? PaymentDate { get; set; } // For Zelle/PayPal
        public string? CheckNumber { get; set; } // For Check
        public string? BankName { get; set; } // For Check
        public DateTimeOffset? CheckDate { get; set; } // For Check
        public decimal Amount { get; set; } // Payment amount




        public string request_type { get; set; }

        public List<ChildrensDetailsRegisterDataModel>? ChildrensDetails { get; set; }
        public SpouseDetailsRegisterData? SpouseDetails { get; set; }
    }
    public class ChildrensDetailsRegisterDataModel
    {
        public string? ChildCountry { get; set; } = null!;
        public string? ChildCity { get; set; } = null!;
        public string? ChildState { get; set; } = null!;
        public string? ChildPhoneNumber { get; set; } = null!;
        public int? ChildYearOfBirth { get; set; }
        public string? ChildUniqueId { get; set; }
        public string? ChildLastName { get; set; }
        public string? ChildEmailAddress { get; set; } = null!;
        public string ChildFirstName { get; set; } = null!;
    }
    public class SpouseDetailsRegisterData
    {
        public string? SpouseEmail { get; set; } = null!;
        public string? SpouseCity { get; set; }
        public string? SpouseState { get; set; }
        public string spousePhoneNumber { get; set; } = null!;
        public string? SpouseCountry { get; set; }
        public string? SpouseHometown { get; set; } = null!;
        public string SpouseFirstName { get; set; } = null!;
        public string? SpouseLastName { get; set; } = null!;
        public int? SpouseYearOfBirth { get; set; }
    }
}