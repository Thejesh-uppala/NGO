using System;
using System.Collections.Generic;

namespace NGO.Data
{
    public partial class UserDetail
    {
        public UserDetail()
        {
            ChildrensDetails = new HashSet<ChildrensDetail>();
            MemberShipTypes = new HashSet<MemberShipType>();
            Payments = new HashSet<Payment>();
        }

        public int Id { get; set; }
        public string? UniqueId { get; set; }
        public int UserId { get; set; }
        public int OrgId { get; set; }
        public int ChapterId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? FamilyName { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string WhatsAppNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string HomeTown { get; set; } = null!;
        public string? SocialMedia { get; set; }
        public string? PreferredBy { get; set; }
        public DateTime? Dob { get; set; }
        public byte[]? Photo { get; set; }
        public string SpouseFirstName { get; set; } = null!;
        public string? SpousePhoneNumber { get; set; }
        public string SpouseLastName { get; set; } = null!;
        public DateTime SpouseDob { get; set; }
        public string SpouseEmail { get; set; } = null!;
        public string? SpouseCity { get; set; }
        public string? SpouseState { get; set; }
        public string? SpouseCountry { get; set; }
        public string SpouseHometown { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Organization Organization { get; set; } = null!;  
        public virtual User User { get; set; } = null!;
        public virtual ICollection<ChildrensDetail> ChildrensDetails { get; set; }
        public virtual ICollection<MemberShipType> MemberShipTypes { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}

