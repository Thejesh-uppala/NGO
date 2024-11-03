namespace NGO.Model
{
    public class AuthModel
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime TokenExpiryDate { get; set; }
        public Guid? RefreshToken { get; set; }
        public List<UserRoleModel> UserRoles { get; set; } = new List<UserRoleModel>();
        // List of roles with IDs and names
        public List<OrganizationModel> Organizations { get; set; } = new List<OrganizationModel>();
        // List of organizations
        public string? Error { get; set; } // Nullable
        public string? PaymentInfo { get; set; } // Nullable
    }
}
