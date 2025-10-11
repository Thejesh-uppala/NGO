namespace NGO.Data
{
    public class EventPaymentModel
    {
        public string PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        public DateTimeOffset? PaymentDate { get; set; }
        public string? CheckNumber { get; set; }
        public string? BankName { get; set; }
        public DateTimeOffset? CheckDate { get; set; }
        public decimal TotalCost { get; set; }

    }
}


