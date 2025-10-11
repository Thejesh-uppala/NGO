namespace NGO.Data
{
    public partial class Payment
    {
        public int Id { get; set; } // Primary key
        public int UserDetailId { get; set; } // Foreign key to UserDetail
        public string PaymentMethod { get; set; } = null!; // Payment method (Zelle, PayPal, Check)
        public string? TransactionId { get; set; } // Transaction ID for Zelle/PayPal
        public DateTimeOffset? PaymentDate { get; set; } // Payment date for Zelle/PayPal
        public string? CheckNumber { get; set; } // Check number for Check payments
        public string? BankName { get; set; } // Bank name for Check payments
        public DateTimeOffset? CheckDate { get; set; } // Check date for Check payments
        public decimal Amount { get; set; } // Payment amount

        public virtual UserDetail UserDetail { get; set; } = null!; // Navigation property
    }
}