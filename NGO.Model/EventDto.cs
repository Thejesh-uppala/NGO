namespace NGO.Model
{

    public class EventDto
    {
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
        public string? EventTimeUtc { get; set; } // Combined UTC start time
        public string? EventLocation { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
        public DateTimeOffset? EventDate { get; set; }
        public DateTimeOffset? RegistrationLastDate { get; set; }
        public List<EventCostDto> EventCosts { get; set; } = new();
    }

    public class EventCostDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int? ParticipantTypeId { get; set; } // New foreign key
        public string Type { get; set; } = string.Empty;
        public string Cost { get; set; } = string.Empty;
        public DateTimeOffset? CreatedDate { get; set; }
    }


}

