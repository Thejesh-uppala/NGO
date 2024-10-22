using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aykan.SRM.Model
{
    public class UspUserReturnModel
    {
        public int Id { get; set; }
        public int? uniqueId { get; set; }
        public int UserId { get; set; }
        public string? OrgId { get; set; } = null;
        public int? ChapterId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? FamilyName { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string WhatsAppNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string HomeTown { get; set; } = null!;
        public string? SocialMedia { get; set; }
        public string? PreferredBy { get; set; }
        public DateTime Dob { get; set; }
        public byte[]? Photo { get; set; } = null!;
        public string SpouseFirstName { get; set; } = null!;
        public string SpouseLastName { get; set; } = null!;
        public DateTime SpouseDob { get; set; }
        public string? SpousePhoneNumber { get; set; } = null!;
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
       // public List<ChildrensDetailsModel>? ChildrensDetails { get; set; }
    }
    //public class ChildrensDetailsModel
    //{
    //    public int Id { get; set; }
    //    public int UserDetailId { get; set; }
    //    public string ChildUniqueId { get; set; }
    //    public string FirstName { get; set; } = null!;
    //    public string LastName { get; set; } = null!;
    //    public string EmialId { get; set; } = null!;
    //    public string PhoneNo { get; set; } = null!;
    //    public DateTime Dob { get; set; }
    //    public bool Resident { get; set; }
    //    public string ResidentCity { get; set; } = null!;
    //    public string ResidentState { get; set; } = null!;
    //    public string ResidentCountry { get; set; } = null!;
    //    // public  UserDetailModel UserDetail { get; set; } = null!;
    //}
}
