using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NGO.Business;

namespace NGO.Web.Controllers
{
    [Route("api/v1/emailData")]
    [ApiController]
    public class ContactUsController : ApiBaseController
    {
        private readonly ContactUsBusiness _contactUsBusiness;

        public ContactUsController(ContactUsBusiness contactUsBusiness)
        {
            _contactUsBusiness = contactUsBusiness;
        }
        [HttpPost]
        [Route("ContactUs")]
        public async Task<IActionResult> SendMail(string contactFormName,string contactFormMessage, string contactFormSubjects,string contactFormEmail)
        {
            await _contactUsBusiness.SendMail(contactFormName, contactFormMessage, contactFormSubjects, contactFormEmail);
            return Ok();
        }
    }
}
