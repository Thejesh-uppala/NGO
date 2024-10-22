using System;
using System.Collections.Generic;

namespace NGO.Data
{
    public partial class Payment
    {
        public int Id { get; set; }
        public int UserDetailId { get; set; }
        public string? PaymentId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? PayPalKey { get; set; }

        public virtual UserDetail UserDetail { get; set; } = null!;
    }
}
