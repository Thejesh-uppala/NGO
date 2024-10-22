using System;
using System.Collections.Generic;

namespace NGO.Data
{
    public partial class ChildrensDetail
    {
        public int Id { get; set; }
        public string? ChildUniqueId { get; set; }
        public int UserDetailId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string EmialId { get; set; } = null!;
        public string PhoneNo { get; set; } = null!;
        public DateTime Dob { get; set; }
        public bool Resident { get; set; }
        public string ResidentCity { get; set; } = null!;
        public string ResidentState { get; set; } = null!;
        public string ResidentCountry { get; set; } = null!;

        public virtual UserDetail UserDetail { get; set; } = null!;
    }
}
