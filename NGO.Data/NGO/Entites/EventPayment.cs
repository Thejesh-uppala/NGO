namespace NGO.Data
{
    public class EventPayment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? TransactionId { get; set; }
        public DateTimeOffset? PaymentDate { get; set; }
        public string? CheckNumber { get; set; }
        public string? BankName { get; set; }
        public DateTimeOffset? CheckDate { get; set; }
        public decimal TotalCost { get; set; }

        public virtual UserDetail UserDetail { get; set; } = null!;
        public virtual EventData EventData { get; set; } = null!;
    }
}
