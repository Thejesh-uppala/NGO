using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGO.Model
{
    public class MemberShipTypeModel
    {
        public int UserDetailId { get; set; }
        public string? MemberShipTypesData { get; set; }
        public long? MemberShipAmount { get; set; }
        public int? ValidityPeriod { get; set; }

        public  UserDetailModel UserDetailModel { get; set; } = null!;
    }
}
