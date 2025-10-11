namespace NGO.Data
{
    public partial class MemberShipType
    {
        public int UserDetailId { get; set; }
        public int? MembershipId { get; set; }

        public virtual UserDetail UserDetail { get; set; } = null!;
        public virtual MembershipData MembershipData { get; set; } = null!;
    }
}
