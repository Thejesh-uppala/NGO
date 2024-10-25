using Microsoft.AspNetCore.Http;


namespace NGO.Model
{
    public class FileModel
    {
        public FileModel()
        {
            SpouseDetails = new SpouseDetailsData();
            ChildrensDetails = new List<ChildrensDetailsDataModel>();
        }
        public IFormFile? Photo { get; set; }
        public string FirstName { get; set; } = null!;
        public int OrgId { get; set; } 
        public string? MemberId { get; set; } = null!;
        public string? FamilyName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime DOB { get; set; }
        public string LastName { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Country { get; set; } = null!;
        public int UserId { get; set; }
        public string? SocialMedia { get; set; } = null!;
        public string? PreferredBy { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public string Refference { get; set; } = null!;
        public string HomeTown { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? WhatsAppNumber { get; set; }
        public byte[]? PhotoData { get; set; }
        public int UserDetailId { get; set; }
        public string? SpouseEmail { get; set; } = null!;
        public string? SpouseCity { get; set; }
        public string? SpouseState { get; set; }
        public string? SpouseCountry { get; set; }
        public string? SpouseHometown { get; set; } = null!;
        public string? SpouseFirstName { get; set; } = null!;
        public string? SpouseLastName { get; set; } = null!;
        public DateTime SpouseDob { get; set; }
        public List<ChildrensDetailsDataModel>? ChildrensDetails { get; set; }
        public SpouseDetailsData? SpouseDetails { get; set; }
        public class ChildrensDetailsDataModel
        {
            public string ChildCountry { get; set; } = null!;
            public string ChildUniqueId { get; set; } = null!;
            public string ChildCity { get; set; } = null!;
            public string ChildState { get; set; } = null!;
            public string ChildPhoneNumber { get; set; } = null!;
            public DateTime ChildDOB { get; set; }
            public string? ChildLastName { get; set; }
            public string ChildEmailAddress { get; set; } = null!;
            public string ChildFirstName { get; set; } = null!;
        }
        public class SpouseDetailsData
        {
            public string SpouseEmail { get; set; } = null!;
            public string? SpouseCity { get; set; }
            public string? SpouseState { get; set; }
            public string? SpouseCountry { get; set; }
            public string SpouseHometown { get; set; } = null!;
            public string SpouseFirstName { get; set; } = null!;
            public string SpouseLastName { get; set; } = null!;
            public string SpousePhoneNumber { get; set; } = null!;
            public DateTime SpouseDob { get; set; }
        }
    }
}
