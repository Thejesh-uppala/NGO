namespace NGO.Data
{
    public partial class UserDetail
    {
        public UserDetail()
        {
            ChildrensDetails = new HashSet<ChildrensDetail>();
            MembershipUserTypes = new HashSet<MemberShipType>();
            Payments = new HashSet<Payment>();
            EventPayments = new HashSet<EventPayment>();
        }

        public int Id { get; set; }
        public string? UniqueId { get; set; }

        public int UserId { get; set; }

        public int OrgId { get; set; }
        public int ChapterId { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string? FamilyName { get; set; }
        public string? IsApproved { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AddInWhatsAppOrTelegram { get; set; }


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

        /// <summary>
        /// iaudit in partial class is not used as its nullable we did
        /// </summary>
        public int? CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public int? ApprovedBy { get; set; } // New field for tracking who approved the membership

        public virtual Organization Organization { get; set; } = null!;

        public virtual User User { get; set; } = null!;
        public virtual UserDetail? Approver { get; set; } // Navigation property for self-referencing

        public virtual ICollection<ChildrensDetail> ChildrensDetails { get; set; }
        public virtual ICollection<MemberShipType> MembershipUserTypes { get; set; }

        public virtual ICollection<EventPayment> EventPayments { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }

        public virtual Address AddressEntity { get; set; }
    }
}

