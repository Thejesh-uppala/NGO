using NGO.Model;

namespace NGO.Data
{
    public partial class PaymentModel
    {
        public int Id { get; set; }
        public int UserDetailId { get; set; }
        public string? PaymentId { get; set; }
        public string? PayerId { get; set; }
        public DateTime? PaymentDate { get; set; }

        public  UserDetailModel UserDetail { get; set; } = null!;
    }
}
