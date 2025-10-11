namespace NGO.Model
{
    public class MemberShipTypeModel
    {
        public int UserDetailId { get; set; }
        public int? MembershipId { get; set; }

        public UserDetailModel UserDetailModel { get; set; } = null!;
    }
}
