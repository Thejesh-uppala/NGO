using System;
using System.Collections.Generic;

namespace NGO.Data
{
    public partial class MemberShipType
    {
        public int Id { get; set; }
        public int UserDetailId { get; set; }
        public string? MemberShipType1 { get; set; }
        public long? MemberShipAmount { get; set; }
        public int? ValidityPeriod { get; set; }

        public virtual UserDetail UserDetail { get; set; } = null!;
    }
}
