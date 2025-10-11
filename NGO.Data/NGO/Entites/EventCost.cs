using System.ComponentModel.DataAnnotations;

namespace NGO.Data
{
    public partial class EventCost
    {
        [Key]
        public int Id { get; set; }
        public int EventId { get; set; }
        public int? ParticipantTypeId { get; set; } // New foreign key
        public string Cost { get; set; } = string.Empty;
        public DateTimeOffset? CreatedDate { get; set; }

        public virtual EventData EventData { get; set; } = null!;
        public virtual ParticipantType ParticipantType { get; set; } = null!; // New navigation property


    }

}


