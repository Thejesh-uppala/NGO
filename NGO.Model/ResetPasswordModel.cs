namespace NGO.Model
{
    public class ResetPasswordModel
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string OldPassword { get; set; }
    }
}
