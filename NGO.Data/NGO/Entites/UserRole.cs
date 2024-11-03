using System;
using System.Collections.Generic;

namespace NGO.Data
{
    public partial class UserRole
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int OrgId { get; set; }

        public virtual User User { get; set; } = null!;    // Navigation property to User
        public virtual Role Role { get; set; } = null!;    // Navigation property to Role
        public virtual Organization Organization { get; set; } = null!; // Navigation property to Organization (add this line)


    }
}
