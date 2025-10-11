namespace NGO.Model
{

    public class UserReminderDto
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTimeOffset? MembershipValidityDate { get; set; }
        public string MembershipStatus { get; set; }
        public string Email { get; set; }
    }


}

