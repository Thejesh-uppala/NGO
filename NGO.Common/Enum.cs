using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGO.Common
{
    public enum EnumLookUp
    {
        Active = 1,
        Suspended = 2,
        New = 3,
        Verified = 4,
        Enrolled = 5,
        Closed = 6

    }
    public enum MemberShipType
    {
        Annual = 1,
        LifeTime = 2
    }
    public enum PaymentInfo
    {
        [Display(Name = "Paid")]
        PAID = 1,

        [Display(Name = "Pending")]
        PENDING = 0,
    }
}
