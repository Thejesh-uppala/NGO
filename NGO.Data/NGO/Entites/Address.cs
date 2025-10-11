namespace NGO.Data
{
    public partial class Address
    {
        public Address()
        {
            UserDetails = new HashSet<UserDetail>();
        }

        public int Id { get; set; }
        public string City { get; set; } = null!;
        public string? State { get; set; } = null!;
        public string? PostalCode { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string? HomeTown { get; set; } = null!;


        public virtual ICollection<UserDetail> UserDetails { get; set; }
        //public virtual UserDetail UserDetail { get; set; } = null!;
    }

}



