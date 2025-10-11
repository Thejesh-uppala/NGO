using System.ComponentModel.DataAnnotations;

namespace NGO.Common
{
    public enum EnumLookUp
    {
        Active = 1,
        InActive = 2

    }
    public enum PaymentInfo
    {
        [Display(Name = "Paid")]
        PAID = 1,

        [Display(Name = "Pending")]
        PENDING = 0,
    }


    public enum PaymentType
    {
        [Display(Name = "Free")]
        FREE,

        [Display(Name = "Paid")]
        PAID,


    }

    public enum PaymentMethodType
    {
        [Display(Name = "Zelle")]
        ZELLE,

        [Display(Name = "PayPal")]
        PAYPAL,

        [Display(Name = "Check")]
        CHECK
    }

    public enum RoleNames
    {


        [Display(Name = "Admin")]
        Admin = 1,

        [Display(Name = "Member")]
        Member = 2,
    }
}

