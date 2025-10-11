using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NGO.Business;
using Swashbuckle.AspNetCore.Annotations;

namespace NGO.Web.Controllers
{
    [Route("api/v1/reminder")]
    [ApiController]
    public class ReminderController : ApiBaseController
    {
        private readonly ReminderBusiness _contactUsBusiness;

        public ReminderController(ReminderBusiness contactUsBusiness)
        {
            _contactUsBusiness = contactUsBusiness;
        }


        [SwaggerOperation(
        Summary = "Remind user(s) by UserId(s) and OrgId",
        Description = "This API remainds a user or multiple users based on UserId(s) in the body and OrgId in the query."
        )]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("RemindUser")]
        public async Task<IActionResult> RemindUser(
            [FromBody] List<string> userEmails,
            [FromQuery] int updatedBy,
            [FromQuery] int orgId)
        {
            if ((userEmails == null || !userEmails.Any()) || orgId <= 0)
            {
                return BadRequest(new { Error = "Invalid UserId(s) or OrgId." });
            }

            try
            {
                await _contactUsBusiness.SendReminderMail(userEmails, updatedBy, orgId);
                return Ok(new { Message = userEmails.Count == 1 ? "User remainded successfully." : "Users remainded successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }



        [SwaggerOperation(
        Summary = "Contact API",
        Description = "This API handles contact form submissions."
    )]
        [Authorize]
        [HttpPost("ContactUs")]
        public async Task<IActionResult> SendMail(
    [FromQuery] string contactFormName,
    [FromQuery] string contactFormMessage,
    [FromQuery] string contactFormSubjects,
    [FromQuery] string contactFormEmail,
    [FromQuery] int orgId)
        {
            await _contactUsBusiness.SendMail(
contactFormName,
contactFormMessage,
contactFormSubjects,
contactFormEmail,
orgId
);
            return Ok(new { message = "Contact request sent successfully" });
        }
    }
}
