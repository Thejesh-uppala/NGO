using System.ComponentModel.DataAnnotations;

namespace NGO.Data
{
    public partial class ParticipantType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        // Navigation property
        public virtual ICollection<UserEventRegistration> UserEventRegistration { get; set; } = new List<UserEventRegistration>();

        public virtual ICollection<GuestEventRegistration> GuestUsersEventRegistration { get; set; } = new List<GuestEventRegistration>();

        public virtual ICollection<EventCost> EventCosts { get; set; } = new List<EventCost>();
    }
}