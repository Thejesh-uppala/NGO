namespace NGO.Data
{
    public partial class MembershipData
    {
        public MembershipData()
        {
            // Initialize the collection to ensure it's not null
            MembershipUserTypes = new HashSet<MemberShipType>();
        }
        public int Id { get; set; }
        public string? MemberShipType { get; set; }
        public long? MemberShipAmount { get; set; }
        public int? ValidityPeriod { get; set; }
        public int? OrgId { get; set; }

        public virtual ICollection<MemberShipType> MembershipUserTypes { get; set; }
        public virtual Organization Organization { get; set; } = null!;


    }


}

