using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGO.Model
{
    public class ChildrensDetailsModel
    {
        public int Id { get; set; }
        public int UserDetailId { get; set; }
        public string ChildUniqueId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string EmialId { get; set; } = null!;
        public string PhoneNo { get; set; } = null!;
        public DateTime Dob { get; set; }
        public bool Resident { get; set; }
        public string ResidentCity { get; set; } = null!;
        public string ResidentState { get; set; } = null!;
        public string ResidentCountry { get; set; } = null!;
       // public  UserDetailModel UserDetail { get; set; } = null!;
    }
}
