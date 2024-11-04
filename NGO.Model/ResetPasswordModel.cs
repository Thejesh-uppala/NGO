namespace NGO.Model
{
    public class ResetPasswordModel
    {
        public int UserId { get; set; }
        public string Password { get; set; } = null!;
        public string OldPassword { get; set; }
    }
}
