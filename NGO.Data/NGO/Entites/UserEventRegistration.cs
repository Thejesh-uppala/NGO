namespace NGO.Data
{
    public partial class UserEventRegistration
    {
        public int Id { get; set; }

        // Explicitly define foreign key properties
        public int EventId { get; set; }
        public int UserId { get; set; }

        // Navigation properties
        public virtual EventData EventData { get; set; }
        public virtual User User { get; set; }
        public int? ParticipantTypeId { get; set; } // Replaced Type

        public string Cost { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public virtual ParticipantType ParticipantType { get; set; } = null!; // New navigation property

    }

}

