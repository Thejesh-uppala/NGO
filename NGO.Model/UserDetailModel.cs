using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NGO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGO.Model
{
    public class UserDetailModel
    {
        public IFormFile? PhotoData { get; set; }
        public int UserId { get; set; }
        public string OrgId { get; set; } = null;
        public DropDownModel? OrgName { get; set; } = null;
        public string MemberId { get; set; } = null;
        public string Address { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? FamilyName { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public DateTime Dob { get; set; }
        public string City { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string WhatsAppNumber { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string HomeTown { get; set; } = null!;
        public string? SocialMedia { get; set; }
        public string? PreferredBy { get; set; }
        public byte[]? Photo { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public string SpouseFirstName { get; set; } = null!;
        public string SpouseLastName { get; set; } = null!;
        public DateTime SpouseDob { get; set; }
        public string SpouseEmail { get; set; } = null!;
        public string? SpouseCity { get; set; }
        public string? SpouseState { get; set; }
        public string? SpouseCountry { get; set; }
        public string SpouseHometown { get; set; } = null!;
        public string SpousePhoneNumber { get; set; } = null!;
        public DateTime? PaymentDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public List<ChildrensDetailsModel> ChildrensDetails { get; set; }

    }

    public class PhotoData
    {
        public int lastModified { get; set; }
        public string name { get; set; } = null!;
        public DateTime lastModifiedDate { get; set; } 
        public int size { get; set; }
        public string type { get; set; }
    }
}
