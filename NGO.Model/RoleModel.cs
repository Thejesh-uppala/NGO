namespace NGO.Model
{
    public class RoleModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

        public List<UserRoleModel> UserRoles { get; set; }
    }
}
