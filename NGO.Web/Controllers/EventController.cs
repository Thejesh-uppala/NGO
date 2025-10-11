using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NGO.Business;
using NGO.Data;
using NGO.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace NGO.Web.Controllers
{
    [Route("api/v1/event")]
    [ApiController]
    public class EventController : ApiBaseController
    {
        private readonly EventBusiness _eventBusiness;
        private readonly CommonBusiness _commonBusiness;
        private readonly IMapper _mapper;
        public EventController(EventBusiness eventBusiness, IMapper mapper, CommonBusiness commonBusiness)
        {
            _eventBusiness = eventBusiness;
            _mapper = mapper;
            _commonBusiness = commonBusiness;
        }

        [SwaggerOperation(
         Summary = "Get All Events Data",
         Description = "This API is used Get All Events"
        )]
        [Authorize]
        [HttpGet]
        [Route("GetAllEvents")]
        public async Task<IActionResult> GetAllEvents(
    [FromQuery] int orgId)
        {
            var events = await _eventBusiness.GetAllEventsAsync(orgId);
            var eventslist = _mapper.Map<IEnumerable<EventDto>>(events);
            return new JsonResult(new { eventslist });
        }

        [SwaggerOperation(
 Summary = "Get All Paginated Events Data",
 Description = "This API is used Get All Paginated Events"
)]
        [Authorize]
        [HttpGet]
        [Route("GetAllPaginatedEvents")]
        public async Task<IActionResult> GetPaginatedEvents(
[FromQuery] int orgId,
[FromQuery] DateTime? startdate,
[FromQuery] DateTime? enddate,
[FromQuery] string? searchValue,
[FromQuery] string? eventtype,
[FromQuery] int page = 1,
[FromQuery] int pageSize = 10)
        {
            var (events, totalRecord) = await _eventBusiness.GetPaginatedEventsAsync(orgId, startdate, enddate, searchValue, page, pageSize, eventtype);
            var eventslist = _mapper.Map<IEnumerable<EventDto>>(events);
            return new JsonResult(new { eventslist, totalRecords = totalRecord });
        }

        [SwaggerOperation(
         Summary = " Get UpcomingEvents Data",
         Description = "This API is used Get All Events"
        )]
        [Authorize]
        [HttpGet]
        [Route("GetUpcomingEvents")]
        public async Task<IActionResult> GetUpcomingEvents([FromQuery] int orgId)
        {
            var events = await _eventBusiness.GetUpcomingEventsAsync(orgId);
            events = events.Where(e => e.EventDate.HasValue).ToList();
            var eventslist = _mapper.Map<IEnumerable<EventDto>>(events);
            return new JsonResult(new { eventslist });

        }

        [SwaggerOperation(
         Summary = "Get Event By Id",
         Description = "This API is used Get Event By Id"
        )]
        [AllowAnonymous]
        [HttpGet]
        [Route("GetEventById")]
        public async Task<IActionResult> GetEventById([FromQuery] int orgId, int EventId)
        {
            var eventData = await _eventBusiness.GetEventByIdAsync(orgId, EventId);

            if (eventData == null)
                return NotFound(new { Message = "Event not found." });
            var eventDto = _mapper.Map<EventDto>(eventData);

            return Ok(eventDto);
        }



        [SwaggerOperation(
         Summary = "Get Registered User for a Event",
         Description = "This API is used Get Registered Users for a Event"
        )]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetRegisteredUsersByEventId")]
        public async Task<IActionResult> GetEventRegistrationDetails([FromQuery] int eventId, [FromQuery] int page = 1,
[FromQuery] int pageSize = 10)
        {
            // Call the business layer to get the data
            var response = await _eventBusiness.GetEventRegistration(eventId, page, pageSize);

            if (response == null)
            {
                return NotFound($"No registration data found for event {eventId}");
            }

            return Ok(response);
        }


        [SwaggerOperation(
           Summary = "Create Event",
           Description = "This API is used Create Event"
          )]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("CreateEvent")]
        public async Task<IActionResult> CreateEvent([FromForm] EventCreateDto eventDto, [FromForm] IFormFile EventPhoto)
        {
            if (eventDto == null)
                return BadRequest(new { Message = "Event data is required." });

            if (eventDto.OrgId == null)
                return BadRequest(new { Message = "Organization ID is required." });

            int orgId = Convert.ToInt32(eventDto.OrgId);

            string? photoUrl = null;
            if (EventPhoto != null)
            {
                photoUrl = await _commonBusiness.UploadToCloudinaryAsync(EventPhoto, orgId);
            }
            List<EventCost> eventCosts;
            try
            {
                eventCosts = JsonConvert.DeserializeObject<List<EventCost>>(eventDto.EventCosts)
                             ?? new List<EventCost>();
            }
            catch (JsonException)
            {
                return BadRequest(new { Message = "Invalid event costs format." });

            }

            if (eventDto.EventAccess.ToLower() == "free" && eventCosts.Any())
            {
                eventCosts.Clear();

            }

            var eventData = _mapper.Map<EventData>(eventDto);
            eventData.EventCosts = eventCosts;
            eventData.EventPhoto = photoUrl;


            string format = "ddd MMM dd yyyy HH:mm:ss 'GMT'zzz '(India Standard Time)'";
            DateTimeOffset eventDateOffset;
            if (DateTimeOffset.TryParseExact(eventDto.EventDate, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out eventDateOffset))
            {
                eventData.EventDate = eventDateOffset.UtcDateTime;  // Convert to UTC if needed
            }
            else
            {
                eventData.EventDate = null; // Handle invalid date format

            }
            DateTimeOffset registrationLastDateOffset;
            if (DateTimeOffset.TryParseExact(eventDto.RegistrationLastDate, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out registrationLastDateOffset))
            {
                eventData.RegistrationLastDate = registrationLastDateOffset.UtcDateTime;  // Convert to UTC if needed
            }
            else
            {
                eventData.RegistrationLastDate = null; // Handle invalid date format

            }

            eventData.Id = 0; // Ensure the database generates the ID
            eventData.EventTimeUtc = (eventDto.EventStartTime).ToString() + '-' + (eventDto.EventEndTime).ToString(); // Store the start time in UTC
            eventData.EventLocation = eventDto.EventLocation;
            var createdEvent = await _eventBusiness.AddEventAsync(eventData);

            if (createdEvent == null)
                return StatusCode(500, new { Message = "Failed to create the event." });
            var createdEventDto = _mapper.Map<EventDto>(createdEvent);

            return CreatedAtAction(nameof(GetEventById), new { orgId = createdEventDto.OrgId, eventId = createdEventDto.Id }, createdEventDto);
        }


        [SwaggerOperation(
             Summary = "Update Event",
             Description = "This API is used to update an event"
         )]
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("UpdateEvent")]
        public async Task<IActionResult> UpdateEvent([FromForm] UpdateEventDto updateeventDto, IFormFile? EventPhoto)
        {
            if (updateeventDto == null)
                return BadRequest(new { Message = "Event data is required." });

            List<EventCost> eventCosts;
            try
            {
                eventCosts = JsonConvert.DeserializeObject<List<EventCost>>(updateeventDto.EventCosts)
                             ?? new List<EventCost>();


            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Invalid event costs format." });
            }



            if (updateeventDto.EventAccess.ToLower() == "free" && eventCosts.Any())
            {
                // Prevent saving event costs if the event is free
                eventCosts.Clear();
            }
            var updatedEventData = _mapper.Map<EventData>(updateeventDto);
            updatedEventData.EventCosts = eventCosts;
            string format = "ddd MMM dd yyyy HH:mm:ss 'GMT'zzz '(India Standard Time)'";
            DateTimeOffset eventDateOffset;
            if (DateTimeOffset.TryParseExact(updateeventDto.EventDate, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out eventDateOffset))
            {
                updatedEventData.EventDate = eventDateOffset.UtcDateTime;  // Convert to UTC if needed
            }
            else
            {
                updatedEventData.EventDate = null; // Handle invalid date format
            }

            DateTimeOffset registrationLastDateOffset;
            if (DateTimeOffset.TryParseExact(updateeventDto.RegistrationLastDate, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out registrationLastDateOffset))
            {
                updatedEventData.RegistrationLastDate = registrationLastDateOffset.UtcDateTime;  // Convert to UTC if needed
            }
            else
            {
                updatedEventData.RegistrationLastDate = null; // Handle invalid date format

            }

            // Map DTO to EventData

            updatedEventData.EventTimeUtc = (updateeventDto.EventStartTime).ToString() + '-' + (updateeventDto.EventEndTime).ToString(); // Store the start time in UTC
            // Send the updatedEventData to the business layer
            var updatedEvent = await _eventBusiness.UpdateEventAsync(updatedEventData, EventPhoto);

            if (updatedEvent == null)
                return StatusCode(500, new { Message = "Failed to update the event." });

            // Return EventId after success
            return Ok(new { Message = "Event updated successfully.", EventId = updatedEvent.Id });
        }



        [SwaggerOperation(
             Summary = "delete Event",
             Description = "This API is used to delete an event"
         )]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            await _eventBusiness.DeleteEventAsync(id);
            return NoContent();
        }


        [SwaggerOperation(
             Summary = "register to a Event",
             Description = "This API is used to register to a event"
         )]
        [Authorize]
        [HttpPost("RegisterEvents")]
        public async Task<IActionResult> RegisterEvents([FromQuery] int orgId, [FromBody] EventRegistrationRequest request)
        {
            try
            {
                await _eventBusiness.RegisterEventsAsync(orgId, request);
                return Ok(new { Message = "Successfully registered for the event." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }



        [SwaggerOperation(
             Summary = "Get Registered Events",
             Description = "This API is used to Get Registered Events"
         )]
        [Authorize]
        [HttpGet("UserRegisteredEvents")]
        public async Task<IActionResult> GetRegisteredEvents(int userId, [FromQuery] int orgId)
        {
            if (userId <= 0 || orgId <= 0)
            {
                return BadRequest("Invalid userId or orgId.");
            }

            try
            {
                var events = await _eventBusiness.GetRegisteredEventsForUserAsync(userId, orgId);
                if (!events.Any())
                {
                    return NotFound("No registered events found for the user.");
                }
                var eventDataList = events.Select(r => r.EventData).ToList(); // Select only EventData from each record

                // Step 3: Map the event data to EventDto
                var registeredEventsList = _mapper.Map<IEnumerable<EventDto>>(eventDataList);

                // Step 4: Return the result as a JSON response
                return new JsonResult(new { registeredEventsList });
            }
            catch (Exception ex)
            {
                // Log the exception (ex)
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the events.");
            }
        }

    }
}
