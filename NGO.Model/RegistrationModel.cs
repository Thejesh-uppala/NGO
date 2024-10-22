using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGO.Model
{
    public class RegistrationModel
    {
        public RegistrationModel()
        {
            SpouseDetails = new SpouseDetailsRegisterData();
            ChildrensDetails = new List<ChildrensDetailsRegisterDataModel>();
        }
        public IFormFile? Photo { get; set; }
        public string FirstName { get; set; } = null!;
        public string Password { get; set; }
        public string? FamilyName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string MemberShipType { get; set; } = null!;
        public DateTime DOB { get; set; }
        public string LastName { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string? SocialMedia { get; set; } = null!;
        public string? PreferredBy { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public string Refference { get; set; } = null!;
        public string HomeTown { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? WhatsAppNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentId { get; set; }
        public string Payer_id { get; set; }
        public byte[]? PhotoData { get; set; }
        public List<ChildrensDetailsRegisterDataModel>? ChildrensDetails { get; set; }
        public SpouseDetailsRegisterData SpouseDetails { get; set; }
    }
    public class ChildrensDetailsRegisterDataModel
    {
        public string ChildCountry { get; set; } = null!;
        public string ChildCity { get; set; } = null!;
        public string ChildState { get; set; } = null!;
        public string ChildPhoneNumber { get; set; } = null!;
        public DateTime ChildDOB { get; set; }
        public string ChildLastName { get; set; }
        public string ChildEmailAddress { get; set; } = null!;
        public string ChildFirstName { get; set; } = null!;
    }
    public class SpouseDetailsRegisterData
    {
        public string SpouseEmail { get; set; } = null!;
        public string? SpouseCity { get; set; }
        public string? SpouseState { get; set; }
        public string? SpouseCountry { get; set; }
        public string SpouseHometown { get; set; } = null!;
        public string SpouseFirstName { get; set; } = null!;
        public string SpouseLastName { get; set; } = null!;
        public DateTime SpouseDob { get; set; }
    }
}