using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NGO.Business;
using NGO.Model;

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

        [AllowAnonymous]
        [HttpPost]
        [Route("RegisterUser")]
        public async Task<IActionResult> RegisterUser(RegistrationModel registrationModel)
        {
            var userId=await _registerBusiness.RegisterUser(registrationModel);
            return Ok(userId);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp(string userName,string email, string password, string phNumber)
        {
            var user=await _registerBusiness.SignUp(userName, email, password, phNumber);
            return Ok(user);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("UploadPhoto")]
        public async Task<IActionResult> UploadPhoto([FromForm] IFormFile photo, string userId)
        {
            await _registerBusiness.UploadPhoto(photo, userId);
            return Ok();
        }
    }
}
