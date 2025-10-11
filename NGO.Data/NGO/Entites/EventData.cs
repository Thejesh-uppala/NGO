namespace NGO.Data
{
    public partial class EventData
    {
        public EventData()
        {
            EventCosts = new HashSet<EventCost>();
            EventPayments = new HashSet<EventPayment>();
        }

        public int Id { get; set; }
        public int OrgId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string EventDesc { get; set; } = string.Empty;
        public string? EventRequirements { get; set; }
        public string? EventPhoto { get; set; }
        public string? EventAttendenceTerms { get; set; }
        public string? EventContactEmail { get; set; }
        public string? EventContactPhone { get; set; }
        public string? AdditionalInfo { get; set; }
        public string? EventAccess { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public DateTimeOffset? EventDate { get; set; }
        public string? EventTimeUtc { get; set; }
        public string? EventLocation { get; set; }
        public DateTimeOffset? RegistrationLastDate { get; set; }
        public virtual ICollection<EventPayment> EventPayments { get; set; }
        public virtual Organization Organization { get; set; } = null!;
        public ICollection<EventCost> EventCosts { get; set; } = new List<EventCost>();
        public virtual ICollection<UserEventRegistration> UserEventRegistration { get; set; }
        public virtual ICollection<GuestEventRegistration> GuestUsersEventRegistration { get; set; }



    }

}