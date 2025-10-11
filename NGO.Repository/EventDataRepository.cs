using Microsoft.EntityFrameworkCore;
using NGO.Common;
using NGO.Common.Models;
using NGO.Data;
using NGO.Repository.Contracts;
using NGO.Repository.Infrastructure;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NGO.Repository.NGO.Sql
{
    public class EventDataRepository : Repository<EventData>, IEventDataRepository
    {
        private readonly NGOContext _context;
        public EventDataRepository(NGOContext nGOContext, ApplicationContext applicationContext) : base(nGOContext, applicationContext)
        {
            _context = nGOContext;
        }

        public async Task RegisterEventsAsync(List<UserEventRegistration> registrations, List<GuestEventRegistration>? guestRegistrations, List<EventPayment> eventPayments)
        {
            _context.UserEventRegistration.AddRange(registrations);

            if (guestRegistrations.Any())
            {
                _context.GuestUsersEventRegistration.AddRange(guestRegistrations);
            }
            if (eventPayments.Any())
            {
                _context.EventPayments.AddRange(eventPayments);
            }
            await _context.SaveChangesAsync();

        }


        public async Task<IEnumerable<EventData>> GetAllEventsAsync(int orgId)
        {

            return await _context.EventData
                .Include(e => e.EventCosts)
                .Where(e => e.OrgId == orgId)

                .ToListAsync();

        }
        public async Task<EventRegistrationResponse> GetEventRegistration(int eventId, int page, int pageSize)
        {
            var registeredUsers = await (from reg in _context.UserEventRegistration
                                         where reg.EventId == eventId
                                         join user in _context.Users on reg.UserId equals user.Id
                                         join participantType in _context.ParticipantTypes on reg.ParticipantTypeId equals participantType.Id into ptJoin
                                         from pt in ptJoin.DefaultIfEmpty()
                                         select new
                                         {
                                             user.Id,
                                             user.Name,
                                             user.Email,
                                             user.ContactNumber,
                                             ParticipantTypeName = pt != null ? pt.Name : PaymentType.FREE.ToString().ToLower()
                                         }).ToListAsync();

            // Step 2: Fetch payment data separately
            var payments = await (from payment in _context.EventPayments
                                  where payment.EventId == eventId
                                  select new
                                  {
                                      payment.UserId,
                                      payment.PaymentMethod,
                                      payment.TransactionId,
                                      payment.PaymentDate,
                                      payment.CheckNumber,
                                      payment.BankName,
                                      payment.CheckDate,
                                      payment.TotalCost
                                  }).ToListAsync();

            // Step 3: Fetch guest users (unchanged, as it’s correct)
            var guestUsers = await (from guest in _context.GuestUsersEventRegistration
                                    where guest.EventId == eventId
                                    join participantType in _context.ParticipantTypes on guest.ParticipantTypeId equals participantType.Id into ptJoin
                                    from pt in ptJoin.DefaultIfEmpty()
                                    select new
                                    {
                                        guest.EventId,
                                        guest.AssociatedWithUserId,
                                        guest.GuestUserFirstName,
                                        guest.GuestUserLastName,
                                        guest.GuestUserPhoneNo,
                                        ParticipantTypeName = pt != null ? pt.Name : PaymentType.FREE.ToString().ToLower()
                                    }).ToListAsync();

            // Step 4: Group registered users by UserId
            var groupedRegisteredUsers = registeredUsers
                .GroupBy(u => u.Id)
                .Select(g => new
                {
                    UserId = g.Key,
                    RegisteredUserName = g.First().Name,
                    RegisteredUserEmail = g.First().Email,
                    RegisteredUserPhone = g.First().ContactNumber,
                    RegisteredUserType = g.GroupBy(r => r.ParticipantTypeName)
                        .ToDictionary(x => x.Key, x => x.Count()),
                    GuestUsers = guestUsers
                        .Where(gu => gu.AssociatedWithUserId == g.Key)
                        .Select(gu => new GuestUser
                        {
                            GuestUserName = gu.GuestUserFirstName + " " + gu.GuestUserLastName,
                            GuestUserPhone = gu.GuestUserPhoneNo,
                            GuestUserType = gu.ParticipantTypeName
                        }).ToList(),
                    PaymentInfo = payments
                        .Where(p => p.UserId == g.Key)
                        .Select(p => new EventPaymentModel
                        {
                            PaymentMethod = p.PaymentMethod,
                            TransactionId = p.PaymentMethod.ToLower() == PaymentMethodType.ZELLE.ToString().ToLower() ||
                                           p.PaymentMethod.ToLower() == PaymentMethodType.PAYPAL.ToString().ToLower()
                                           ? p.TransactionId : null,
                            PaymentDate = p.PaymentMethod.ToLower() == PaymentMethodType.ZELLE.ToString().ToLower() ||
                                          p.PaymentMethod.ToLower() == PaymentMethodType.PAYPAL.ToString().ToLower()
                                          ? p.PaymentDate : null,
                            CheckNumber = p.PaymentMethod.ToLower() == PaymentMethodType.CHECK.ToString().ToLower()
                                          ? p.CheckNumber : null,
                            BankName = p.PaymentMethod.ToLower() == PaymentMethodType.CHECK.ToString().ToLower()
                                       ? p.BankName : null,
                            CheckDate = p.PaymentMethod.ToLower() == PaymentMethodType.CHECK.ToString().ToLower()
                                        ? p.CheckDate : null,
                            TotalCost = p.TotalCost
                        }).FirstOrDefault() // Take the first payment (or adjust logic if multiple payments are valid)
                }).ToList();

            // Step 5: Apply pagination
            var paginatedData = groupedRegisteredUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RegisteredUser
                {
                    UserID = r.UserId,
                    RegisteredUserName = r.RegisteredUserName ?? string.Empty,
                    RegisteredUserEmail = r.RegisteredUserEmail ?? string.Empty,
                    RegisteredUserPhone = r.RegisteredUserPhone ?? string.Empty,
                    RegisteredUserType = r.RegisteredUserType,
                    GuestUsers = r.GuestUsers,
                    PaymentInfo = r.PaymentInfo
                })
                .ToList();

            var totalRecords = groupedRegisteredUsers.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Step 6: Build response
            var response = new EventRegistrationResponse
            {
                EventId = eventId,
                RegisteredUsers = paginatedData,
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages
            };

            return response;
        }




        public async Task<(IEnumerable<EventData>, int TotalRecords)> GetFilteredEventsAsync(int orgId, DateTime? startdate, DateTime? enddate, string? searchValue, int page, int pageSize, string? eventtype)
        {
            DateTime currentDate = DateTime.UtcNow.Date;
            // Start with the base query
            var query = _context.EventData
                .Include(e => e.EventCosts)
                .Where(e => e.OrgId == orgId);

            // Apply date range filter only if both dates are provided
            if (startdate.HasValue && enddate.HasValue)
            {
                query = query.Where(e => e.EventDate >= startdate.Value && e.EventDate <= enddate.Value);
            }
            // Apply search filter if searchValue is provided
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                string searchLower = searchValue.ToLower(); // Convert searchValue to lowercase
                query = query.Where(e =>
                    e.EventTitle.ToLower().Contains(searchLower) ||
                    e.EventLocation.ToLower().Contains(searchLower));
            }
            if (!string.IsNullOrWhiteSpace(eventtype) && eventtype.ToLower() != "")
            {
                string eventTypeLower = eventtype.ToLower();
                query = query.Where(e => e.EventAccess.ToLower() == eventTypeLower);
            }
            var totalRecords = await query.CountAsync();
            var data = await query
           .Skip((page - 1) * pageSize)
           .Take(pageSize)
           .ToListAsync();

            return (data, totalRecords);

        }
        public async Task<IEnumerable<UserEventRegistration>> GetRegisteredEventsAsync(int userId, int orgId)
        {
            try
            {
                var events = await _context.UserEventRegistration
                    .Include(r => r.EventData)
                    .Where(r => r.UserId == userId && r.EventData.OrgId == orgId)
                    .ToListAsync();

                // Remove duplicates based on EventId
                return events
                    .GroupBy(r => r.EventData.Id) // Group by EventId
                    .Select(group => group.First())   // Take the first record from each group
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log the full exception details
                throw; // Re-throw to maintain original error handling
            }
        }


        public async Task<EventData?> GetEventByIdAsync(int orgId, int eventId)
        {
            var query = _context.EventData
                 .Include(e => e.EventCosts)
                     .ThenInclude(ec => ec.ParticipantType)
                 .Where(e => e.OrgId == orgId && e.Id == eventId);



            return await query.FirstOrDefaultAsync();
        }

        public async Task<EventData> AddEventAsync(EventData eventData)
        {
            try
            {
                await _context.Set<EventData>().AddAsync(eventData);
                await _context.SaveChangesAsync();

                return eventData;
            }

            catch (Exception ex)
            {
                // Log the full exception details
                throw; // Re-throw to maintain original error handling
            }
        }

        public async Task<EventData?> UpdateEventAsync(EventData updatedEvent)
        {
            // Retrieve the existing event with its EventCosts
            var existingEvent = await _context.EventData
                .Include(e => e.EventCosts) // Load existing EventCosts
                .FirstOrDefaultAsync(e => e.Id == updatedEvent.Id);

            if (existingEvent == null)
            {
                return null; // Event not found
            }

            // Update scalar properties
            _context.Entry(existingEvent).CurrentValues.SetValues(updatedEvent);
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {

            }
            // Save changes to the database

            return existingEvent; // Return the updated event
        }

        public async Task DeleteEventAsync(int id)
        {
            var eventData = await _context.EventData
                .Include(e => e.EventCosts) // Include related data
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventData != null)
            {
                // Remove dependent UserEventRegistrations
                var userRegistrations = _context.UserEventRegistration
                    .Where(uer => uer.EventId == id);
                _context.UserEventRegistration.RemoveRange(userRegistrations);

                // Remove dependent GuestUsersEventRegistrations
                var guestRegistrations = _context.GuestUsersEventRegistration
                    .Where(gu => gu.EventId == id);
                _context.GuestUsersEventRegistration.RemoveRange(guestRegistrations);

                var paymentInformation = _context.EventPayments
                    .Where(gu => gu.EventId == id);
                _context.EventPayments.RemoveRange(paymentInformation);
                // Remove associated EventCosts
                if (eventData.EventCosts.Any())
                {
                    _context.EventCosts.RemoveRange(eventData.EventCosts);
                }

                // Remove EventData record
                _context.EventData.Remove(eventData);

                // Save changes
                await _context.SaveChangesAsync();
            }
        }


    }
}
