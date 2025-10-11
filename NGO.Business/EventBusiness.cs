using System.Globalization;
using System.Threading.Channels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using NGO.Common;
using NGO.Data;
using NGO.Model;
using NGO.Repository;
using NGO.Repository.Contracts;

namespace NGO.Business
{
    public class EventBusiness
    {
        private readonly IUserRepository _userRepository;
        private readonly AppSettings _appsettings;
        private readonly IMapper _mapper;
        private readonly IUserRolesRepository _userRolesRepository;
        private readonly IOrgRepository _organizationRepository;
        private readonly IUserDetailRepository _userDetailRepository;
        private readonly IMembershipUserTypesRepository _memberShipTypesRepository;
        private readonly IChildrensRepository _childrensRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly IMatrimonialDataRepository _matrimonialDataRepository;
        private readonly IMatrimonialPhotoDataRepository _matrimonialPhotoDataRepository;
        private readonly IEventDataRepository _eventRepository;
        private readonly SMTPEmailProvider _smtpEmailProvider;
        private readonly PasswordHandler _passwordHandler;
        private readonly UserBusiness _userBusiness;
        private readonly CommonBusiness _commonBusiness;
        private readonly Channel<EmailDataModel> _emailChannel;


        public EventBusiness(Channel<EmailDataModel> emailChannel, IUserRepository userRepository, AppSettings appsettings, IMatrimonialDataRepository matrimonialDataRepository, IMatrimonialPhotoDataRepository matrimonialPhotoDataRepository, IMapper mapper, IUserRolesRepository userRolesRepository, IOrgRepository organizationRepository, IUserDetailRepository userDetailRepository, IMembershipUserTypesRepository memberShipTypesRepository, IChildrensRepository childrensRepository, IRoleRepository roleRepository, IEventDataRepository eventRepository, SMTPEmailProvider sMTPEmailProvider, PasswordHandler passwordHandler, UserBusiness userBusiness, IUserAddressRepository userAddressRepository, CommonBusiness commonBusiness)
        {
            _userRepository = userRepository;
            _appsettings = appsettings;
            _matrimonialDataRepository = matrimonialDataRepository;
            _matrimonialPhotoDataRepository = matrimonialPhotoDataRepository;
            _mapper = mapper;
            _userRolesRepository = userRolesRepository;
            _organizationRepository = organizationRepository;
            _userDetailRepository = userDetailRepository;
            _memberShipTypesRepository = memberShipTypesRepository;
            _childrensRepository = childrensRepository;
            _roleRepository = roleRepository;
            _eventRepository = eventRepository;
            _smtpEmailProvider = sMTPEmailProvider;
            _passwordHandler = passwordHandler;
            _userBusiness = userBusiness;
            _userAddressRepository = userAddressRepository;
            _commonBusiness = commonBusiness;
            _emailChannel = emailChannel;
        }

        public async Task<IEnumerable<EventData>> GetAllEventsAsync(int orgId)
        {
            return await _eventRepository.GetAllEventsAsync(orgId);
        }



        public async Task<(IEnumerable<EventData>, int TotalRecords)> GetPaginatedEventsAsync(
            int orgId,
            DateTime? startdate = null,
            DateTime? enddate = null,
            string? searchValue = null,
            int page = 1,
            int pageSize = 10,
            string? eventtype = null)
        {
            // Ensure that filters are applied before proceeding
            if (startdate.HasValue || enddate.HasValue || !string.IsNullOrEmpty(searchValue))
            {
                string format = "ddd MMM dd yyyy HH:mm:ss 'GMT'zzz '(India Standard Time)'";

                // Convert startdate to UTC if it has a value
                if (startdate.HasValue)
                {
                    DateTimeOffset startdateOffset;
                    if (DateTimeOffset.TryParseExact(startdate.Value.ToString("ddd MMM dd yyyy HH:mm:ss 'GMT'zzz '(India Standard Time)'"),
                                                     format,
                                                     CultureInfo.InvariantCulture,
                                                     DateTimeStyles.AssumeUniversal,
                                                     out startdateOffset))
                    {
                        startdate = startdateOffset.UtcDateTime; // Convert to UTC
                    }
                }

                // Convert enddate to UTC if it has a value
                if (enddate.HasValue)
                {
                    DateTimeOffset enddateOffset;
                    if (DateTimeOffset.TryParseExact(enddate.Value.ToString("ddd MMM dd yyyy HH:mm:ss 'GMT'zzz '(India Standard Time)'"),
                                                     format,
                                                     CultureInfo.InvariantCulture,
                                                     DateTimeStyles.AssumeUniversal,
                                                     out enddateOffset))
                    {
                        enddate = enddateOffset.UtcDateTime; // Convert to UTC
                    }
                }


            }
            // Fetch filtered events
            return await _eventRepository.GetFilteredEventsAsync(orgId, startdate, enddate, searchValue, page, pageSize, eventtype);

        }


        public async Task<IEnumerable<EventData>> GetUpcomingEventsAsync(int orgId)
        {   // Retrieve all events for the organization
            var events = await _eventRepository.GetAllEventsAsync(orgId);
            return events
                .Where(e => e.EventDate.HasValue && e.EventDate >= DateTime.UtcNow.Date) // Ensure EventDate exists and is in the future
                .OrderBy(e => e.EventDate);
        }

        public async Task<EventData?> GetEventByIdAsync(int orgId, int EventId)
        {
            var eventData = await _eventRepository.GetEventByIdAsync(orgId, EventId);

            return eventData == null ? null : _mapper.Map<EventData>(eventData);
        }

        public async Task<EventData?> UpdateEventAsync(EventData updatedEventData, IFormFile EventPhoto)
        {
            var existingEvent = await GetEventByIdAsync(updatedEventData.OrgId, updatedEventData.Id);
            if (existingEvent == null)
            {
                return null; // Event not found
            }

            existingEvent.EventCosts = updatedEventData.EventCosts;

            // Handle EventPhoto logic
            if (EventPhoto != null)
            {
                // If no EventPhoto is provided, upload it to Cloudinary
                existingEvent.EventPhoto = await _commonBusiness.UploadToCloudinaryAsync(EventPhoto, updatedEventData.OrgId);
            }
            else if (EventPhoto == null)
            {
                // If EventPhoto is provided, retain the existing EventPhoto or save new one
                existingEvent.EventPhoto = existingEvent.EventPhoto; // (existing value, or set the new uploaded value if needed)
            }
            // Check and update other fields
            if (existingEvent.EventTitle != updatedEventData.EventTitle)
            {
                existingEvent.EventTitle = updatedEventData.EventTitle;
            }

            if (existingEvent.EventDesc != updatedEventData.EventDesc)
            {
                existingEvent.EventDesc = updatedEventData.EventDesc;
            }

            if (existingEvent.EventRequirements != updatedEventData.EventRequirements)
            {
                existingEvent.EventRequirements = updatedEventData.EventRequirements;
            }

            if (existingEvent.EventAttendenceTerms != updatedEventData.EventAttendenceTerms)
            {
                existingEvent.EventAttendenceTerms = updatedEventData.EventAttendenceTerms;
            }

            if (existingEvent.EventContactEmail != updatedEventData.EventContactEmail)
            {
                existingEvent.EventContactEmail = updatedEventData.EventContactEmail;
            }

            if (existingEvent.EventContactPhone != updatedEventData.EventContactPhone)
            {
                existingEvent.EventContactPhone = updatedEventData.EventContactPhone;
            }

            if (existingEvent.AdditionalInfo != updatedEventData.AdditionalInfo)
            {
                existingEvent.AdditionalInfo = updatedEventData.AdditionalInfo;
            }

            if (existingEvent.EventAccess != updatedEventData.EventAccess)
            {
                existingEvent.EventAccess = updatedEventData.EventAccess;
            }

            if (existingEvent.CreatedDate != updatedEventData.CreatedDate)
            {
                existingEvent.CreatedDate = updatedEventData.CreatedDate;
            }

            if (existingEvent.EventDate != updatedEventData.EventDate)
            {
                existingEvent.EventDate = updatedEventData.EventDate;
            }

            if (existingEvent.EventTimeUtc != updatedEventData.EventTimeUtc)
            {
                existingEvent.EventTimeUtc = updatedEventData.EventTimeUtc;
            }

            if (existingEvent.EventLocation != updatedEventData.EventLocation)
            {
                existingEvent.EventLocation = updatedEventData.EventLocation;
            }
            if (existingEvent.RegistrationLastDate != updatedEventData.RegistrationLastDate)
            {
                existingEvent.RegistrationLastDate = updatedEventData.RegistrationLastDate;
            }
            // Save the updated event to the repository
            await _eventRepository.UpdateEventAsync(existingEvent);

            // Return the updated event after saving
            return existingEvent;
            // Proceed with the update logic (without photo)
            // Update the event data in the repository
            //var updatedEvent = await _eventRepository.UpdateEventAsync(updatedEventData);
            //return updatedEvent;
        }





        public async Task<EventData> AddEventAsync(EventData eventData)
        {
            eventData.CreatedDate = DateTime.UtcNow;
            foreach (var eventCost in eventData.EventCosts)
            {
                eventCost.CreatedDate = DateTime.UtcNow;
            }
            var createdEvent = await _eventRepository.AddEventAsync(eventData);
            if (createdEvent != null)
            {

                var formattedEventTime = _commonBusiness.FormatEventTime(eventData.EventTimeUtc ?? "N/A");


                var (brandImageUrl, brandColor, orgName, orgWelMsg, brandLogo, primaryEmail, matrimonyEmail, eventEmail, orgHeadEmail) =
                await _userBusiness.GetBrandDetailsAsync(eventData.OrgId);

                string emailBodyTemplate = await _commonBusiness.GetEmailTemplateFromCloudinaryAsync("admin", "event-creation-info.html", eventData.OrgId);



                var emailBody = emailBodyTemplate

                        .Replace("{EventTitle}", eventData.EventTitle ?? "N/A")
                        .Replace("{EventDesc}", eventData.EventDesc ?? "N/A")
                        .Replace("{EventDate}", eventData.EventDate?.ToString("yyyy-MM-dd") ?? "N/A")
                        .Replace("{EventTimeUtc}", formattedEventTime ?? "N/A")
                        .Replace("{EventLocation}", eventData.EventLocation ?? "N/A")
                        .Replace("{EventContactEmail}", eventData.EventContactEmail ?? "N/A")
                        .Replace("{EventContactPhone}", eventData.EventContactPhone ?? "N/A")
                        .Replace("{EventRequirements}", eventData.EventRequirements ?? "N/A")
                        .Replace("{EventAttendenceTerms}", eventData.EventAttendenceTerms ?? "N/A")
                        .Replace("{AdditionalInfo}", eventData.AdditionalInfo ?? "N/A")
                        .Replace("{OrgName}", orgName ?? "N/A");

                var emailModel = new EmailDataModel
                {
                    To = new List<string> { eventData.EventContactEmail ?? primaryEmail },
                    Subject = "New Event Created",
                    Data = emailBody,
                    OrgId = eventData.OrgId
                };
                await _emailChannel.Writer.WriteAsync(emailModel);

            }
            return createdEvent;
        }

        public async Task DeleteEventAsync(int id)
        {
            await _eventRepository.DeleteEventAsync(id);
        }
        public async Task<EventRegistrationResponse> GetEventRegistration(int eventId, int page, int pageSize)
        {
            // Fetch the registered users and their guest users from the repository
            var registeredUsers = await _eventRepository.GetEventRegistration(eventId, page, pageSize);

            // If no registered users are found, return null or empty response
            if (registeredUsers == null)
            {
                return null;
            }

            return registeredUsers;
        }
        public async Task RegisterEventsAsync(int orgId, EventRegistrationRequest request)
        {
            var eventData = await _eventRepository.GetEventByIdAsync(orgId, request.EventId);
            if (eventData.RegistrationLastDate <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Event registration has been closed.");

            }
            var registrations = new List<UserEventRegistration>();
            var guestRegistrations = new List<GuestEventRegistration>();
            var eventPayments = new List<EventPayment>();

            if (request.Access == "Paid")
            {
                foreach (var charge in request.UserPaidDetails)
                {
                    registrations.Add(new UserEventRegistration
                    {
                        EventId = request.EventId,
                        UserId = request.UserId,
                        ParticipantTypeId = charge.ParticipantTypeId,
                        Cost = charge.Cost,
                        CreatedDate = DateTime.UtcNow
                    });
                }
                if (request.Guests != null && request.Guests.Any())
                {
                    foreach (var guest in request.Guests)
                    {
                        guestRegistrations.Add(new GuestEventRegistration
                        {
                            EventId = request.EventId,
                            AssociatedWithUserId = request.UserId,
                            GuestUserFirstName = guest.FirstName,
                            GuestUserLastName = guest.LastName,
                            GuestUserPhoneNo = guest.PhoneNo,
                            ParticipantTypeId = guest.ParticipantTypeId,
                            Cost = guest.Cost,
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                }
                if (request.PaymentDetails != null)
                {
                    var payment = new EventPayment
                    {
                        UserId = request.UserId,
                        EventId = request.EventId,
                        PaymentMethod = request.PaymentDetails.PaymentMethod,
                        TransactionId = (request.PaymentDetails.PaymentMethod.ToLower() == PaymentMethodType.ZELLE.ToString().ToLower() || request.PaymentDetails.PaymentMethod.ToLower() == PaymentMethodType.PAYPAL.ToString().ToLower()) ? request.PaymentDetails.TransactionId : null,
                        PaymentDate = (request.PaymentDetails.PaymentMethod.ToLower() == PaymentMethodType.ZELLE.ToString().ToLower() || request.PaymentDetails.PaymentMethod.ToLower() == PaymentMethodType.PAYPAL.ToString().ToLower()) ? request.PaymentDetails.PaymentDate : null,
                        CheckNumber = (request.PaymentDetails.PaymentMethod.ToLower() == PaymentMethodType.CHECK.ToString().ToLower()) ? request.PaymentDetails.CheckNumber : null,
                        BankName = (request.PaymentDetails.PaymentMethod.ToLower() == PaymentMethodType.CHECK.ToString().ToLower()) ? request.PaymentDetails.BankName : null,
                        CheckDate = (request.PaymentDetails.PaymentMethod.ToLower() == PaymentMethodType.CHECK.ToString().ToLower()) ? request.PaymentDetails.CheckDate : null,
                        TotalCost = request.PaymentDetails.Amount,
                    };

                    eventPayments.Add(payment);
                }
            }
            else if (request.Access == "Free")
            {
                registrations.Add(new UserEventRegistration
                {
                    EventId = request.EventId,
                    UserId = request.UserId,
                    Cost = "0",
                    CreatedDate = DateTime.UtcNow
                });
                guestRegistrations = new List<GuestEventRegistration>();
            }
            else
            {
                throw new ArgumentException("Invalid access type.");
            }

            await _eventRepository.RegisterEventsAsync(registrations, guestRegistrations, eventPayments);


            var (brandImageUrl, brandColor, orgName, orgWelMsg, brandLogo, primaryEmail, matrimonyEmail, eventEmail, orgHeadEmail) =
       await _userBusiness.GetBrandDetailsAsync(orgId);

            string emailBodyTemplate = await _commonBusiness.GetEmailTemplateFromCloudinaryAsync("user", "event-registration-notify.html", orgId);



            var eventDto = _mapper.Map<EventDto>(eventData);
            var user = await _userRepository.GetUserByIdAndOrg(request.UserId, orgId);

            var emailBody = emailBodyTemplate
        .Replace("{OrgName}", orgName ?? "N/A")
        .Replace("{UserName}", user.Name ?? "N/A")
        .Replace("{EventName}", eventDto.EventTitle)
.Replace("{EventDate}", eventDto.EventDate?.ToString("yyyy-MM-dd") ?? "")
        .Replace("{EventLocation}", eventDto.EventLocation)
        .Replace("{SupportEmail}", primaryEmail ?? "NA");

            if (request.Guests?.Any() == true)
            {
                var guestDetailsHtml = string.Join("", request.Guests.Select(guest =>
                    $"<li>{guest.FirstName} {guest.LastName} ({guest.PhoneNo})</li>"));
                emailBody = emailBody.Replace("{GuestDetails}", guestDetailsHtml);
            }
            else
            {
                emailBody = emailBody.Replace("{GuestDetails}", "No guests registered.");
            }

            // Email data model
            var emailModel = new EmailDataModel
            {
                To = new List<string> { user.Email },
                Subject = $"Registration Confirmation for {eventDto.EventTitle}",
                Data = emailBody,
                OrgId = orgId
            };

            await _emailChannel.Writer.WriteAsync(emailModel);





        }


        public async Task<IEnumerable<UserEventRegistration>> GetRegisteredEventsForUserAsync(int userId, int orgId)
        {
            return await _eventRepository.GetRegisteredEventsAsync(userId, orgId);
        }



    }
}
