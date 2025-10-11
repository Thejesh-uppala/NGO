namespace NGO.Data.NGO.Entites
{
    public class MembershipDataModel
    {

        public int Id { get; set; }
        public string? MemberShipType { get; set; }
        public long? MemberShipAmount { get; set; }
        public int? ValidityPeriod { get; set; }

    }

    public class ApprovalRequest
    {
        public List<int> UserIds { get; set; }
        public int UserId { get; set; }
    }

}

