namespace NGO.Model
{
    public class UpdateEventDto
    {
        public int Id { get; set; }
        public int OrgId { get; set; }
        public string? EventTitle { get; set; }
        public string? EventDesc { get; set; }
        public string? EventRequirements { get; set; }
        public string? EventAttendenceTerms { get; set; }
        public string? EventContactEmail { get; set; }
        public string? EventContactPhone { get; set; }
        public string? AdditionalInfo { get; set; }
        public string? EventAccess { get; set; }

        public string? EventDate { get; set; } // Event date (YYYY-MM-DD)
        public string? EventStartTime { get; set; } // Start time (HH:mm)
        public string? EventEndTime { get; set; } // End time (HH:mm)
        public string? EventLocation { get; set; }
        public string? RegistrationLastDate { get; set; }

        // JSON string of event costs
        public string? EventCosts { get; set; }
    }
}
