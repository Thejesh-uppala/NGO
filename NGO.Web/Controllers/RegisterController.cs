using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NGO.Business;
using NGO.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace NGO.Web.Controllers
{
    [Route("api/v1/register")]
    [ApiController]
    public class RegisterController : ApiBaseController
    {
        private readonly RegisterBusiness _registerBusiness;

        public RegisterController(RegisterBusiness registerBusiness)
        {
            _registerBusiness = registerBusiness;
        }

        [SwaggerOperation(
           Summary = "register api",
           Description = "This API is used to register a member"
        )]
        [Authorize]
        [HttpPost]
        [Route("RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromQuery] int orgId, [FromForm] string registrationModel, [FromForm] IFormFile? photo)
        {
            var registrationModelObj = JsonConvert.DeserializeObject<RegistrationModel>(registrationModel);
            if (registrationModelObj == null)
            {
                return BadRequest("Unable to parse registration model.");
            }

            if (orgId <= 0)
            {
                return BadRequest("Invalid OrgId.");
            }





            try
            {

                var result = await _registerBusiness.RegisterUser(orgId, registrationModelObj, photo);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }


        [SwaggerOperation(
           Summary = "signup api",
           Description = "This API is used to signup"
        )]
        [AllowAnonymous]
        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp([FromQuery] int orgId, string email, string password, string? phNumber, string firstName, string? lastName, string? address, string city, string country, string state, int chapterId)
        {

            var user = await _registerBusiness.SignUp(email, password, phNumber, orgId, firstName, lastName, address, city, country, state, chapterId);
            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("SaveOrgConfig")]
        public async Task<IActionResult> SaveConfig([FromBody] OrganizationConfigInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _registerBusiness.SaveConfigAsync(inputModel);
                return Ok(new { Message = $"Configuration saved successfully for OrgId {inputModel.OrgId}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while saving configuration.", Error = ex.Message });
            }
        }

    }
}
