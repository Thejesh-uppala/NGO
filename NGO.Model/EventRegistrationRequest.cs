namespace NGO.Model
{
    public class EventRegistrationRequest
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string Access { get; set; } // "Paid" or "Free"
        public List<UserPaidDetails> UserPaidDetails { get; set; } = new List<UserPaidDetails>();
        public List<GuestDetail>? Guests { get; set; } = new List<GuestDetail>();
        public EventPaymentDetails? PaymentDetails { get; set; } // New Payment Details

    }

    public class GuestDetail
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNo { get; set; } = null!;
        public int ParticipantTypeId { get; set; } // Replaced Type
        public string Cost { get; set; } = null!;
    }

    public class UserPaidDetails
    {
        public int ParticipantTypeId { get; set; } // Replaced Type
        public string Cost { get; set; }
    }


    public class EventPaymentDetails
    {
        public string PaymentMethod { get; set; } = null!; // Zelle, PayPal, Check
        public string? TransactionId { get; set; } // For Zelle/PayPal
        public DateTimeOffset? PaymentDate { get; set; } // For Zelle/PayPal
        public string? CheckNumber { get; set; } // For Check
        public string? BankName { get; set; } // For Check
        public DateTimeOffset? CheckDate { get; set; } // For Check
        public decimal Amount { get; set; } // Payment amount
    }

}

