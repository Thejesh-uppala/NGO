namespace NGO.Model
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ContactNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Status { get; set; }
        public string PaymentInfo { get; set; }
        public int? UnsuccessfulLoginAttempts { get; set; }
        public DateTime? LastLogin { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public List<UserDetailModel> UserDetails { get; set; }
    }
}