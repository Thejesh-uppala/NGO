using NGO.Data;

public class GuestUser
{
    public string GuestUserName { get; set; }
    public string GuestUserPhone { get; set; }
    public string GuestUserType { get; set; }
}

public class RegisteredUser
{
    public int UserID { get; set; }
    public string RegisteredUserName { get; set; }
    public string RegisteredUserEmail { get; set; }
    public string RegisteredUserPhone { get; set; }
    public Dictionary<string, int> RegisteredUserType { get; set; }
    public List<GuestUser> GuestUsers { get; set; }
    public EventPaymentModel? PaymentInfo { get; set; } // New field for payment details

}

public class EventRegistrationResponse
{
    public int EventId { get; set; }

    public List<RegisteredUser> RegisteredUsers { get; set; }

    public int TotalRecords { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}