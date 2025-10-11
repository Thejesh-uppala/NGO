
using NGO.Data;

namespace NGO.Repository.Contracts
{
    public interface IEventDataRepository : IRepository<EventData>
    {
        Task<IEnumerable<EventData>> GetAllEventsAsync(int orgId);

        Task<(IEnumerable<EventData>, int TotalRecords)> GetFilteredEventsAsync(int orgId, DateTime? startdate, DateTime? enddate, string? searchValue, int page, int pageSize, string? eventype);
        Task<EventData?> GetEventByIdAsync(int orgId, int EventId);
        Task<EventData> AddEventAsync(EventData eventData);

        Task<EventRegistrationResponse> GetEventRegistration(int eventId, int page, int pageSize);
        Task<EventData> UpdateEventAsync(EventData eventData);
        Task DeleteEventAsync(int id);
        Task RegisterEventsAsync(List<UserEventRegistration> registrations, List<GuestEventRegistration>? guestRegistrations, List<EventPayment> eventPayments);

        Task<IEnumerable<UserEventRegistration>> GetRegisteredEventsAsync(int userId, int orgId);
    }

}



