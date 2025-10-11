using System.ComponentModel.DataAnnotations;

namespace NGO.Data
{
    public partial class GuestEventRegistration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public int AssociatedWithUserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string GuestUserFirstName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string GuestUserLastName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string GuestUserPhoneNo { get; set; } = null!;

        public int? ParticipantTypeId { get; set; } // Replaced Type


        [Required]
        [MaxLength(10)]
        public string Cost { get; set; } = null!;

        public DateTimeOffset? CreatedDate { get; set; }

        // Navigation properties
        public virtual EventData EventData { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ParticipantType ParticipantType { get; set; } = null!; // New navigation property

    }
}


