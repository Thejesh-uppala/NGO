using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using NGO.Business;
using NGO.Common;
using NGO.Model;

namespace NGO.Web.Controllers
{
    [Route("api/v1/user")]
    [ApiController]
    public class UserController : ApiBaseController
    {
        private readonly UserBusiness _userBusiness;


        public UserController(UserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
           
            var authModel = await _userBusiness.GetUserDetails(loginModel);
            if (authModel.Error == null)
            {
                await _userBusiness.PopulateJwtTokenAsync(authModel);
                await _userBusiness.UpdatelastLogin(authModel);
            }
            return Ok(authModel);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("SendOTP")]
        public async Task<HttpResponseMessage> SendOTPData(string emailId)
        {
            var response = await _userBusiness.ForGotPasswordSendOTP(emailId);
            return response;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string emailId, string newPassword)
        {
            await _userBusiness.ForgotPassword(emailId, newPassword);
            return Ok();
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAllUser")]
        public async Task<JsonResult> GetAllUser()
        {
            var userslist = await _userBusiness.GetAllUser();
            return new JsonResult(new { userslist });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetCurrentUserDetails")]
        public async Task<IActionResult> GetCurrentUserDetails(int userId)
        {
            var userslist = await _userBusiness.GetCurrentUserDetails(userId);
            return new JsonResult(new { userslist });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetAllChapters")]
        public async Task<IActionResult> GetAllChapters()
        {
            var response = await _userBusiness.GetAllChapters();
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetAllOrganizations")]
        public async Task<IActionResult> GetAllOrganizations()
        {
            var response = await _userBusiness.GetAllOrganizations();
            return Ok(response);
        }
        [HttpGet]
        [Route("GetPaymentDetails")]
        public async Task<IActionResult> GetPaymentDetails(int userId)
        {
            var response = await _userBusiness.GetPaymentDetails(userId);
            return Ok(response);
        }
        [HttpGet]
        [Route("GetCurrentUserProfile")]
        public async Task<IActionResult> GetCurrentUserProfile(int userId)
        {
            var userslist = await _userBusiness.GetCurrentUserProfile(userId);
            return new JsonResult(new { userslist });
        }
        [HttpPost]
        [Route("UpdateUser")]
        public async Task<JsonResult> UpdateUser([FromBody] FileModel userDetailModel)
        {
            var userslist = await _userBusiness.UpdateUser(userDetailModel);
            return new JsonResult(new { userslist });
        }
        [HttpPost]
        [Route("UploadPhoto")]
        public async Task<IActionResult> UploadPhoto([FromForm] IFormFile photo,string userId)
        {
            var photoData=await _userBusiness.UploadPhoto(photo, userId);
            return Ok(photoData);
        }
        [HttpPost]
        [Route("UserContactEmailRem")]
        public async Task<IActionResult> UserContactEmailRem(int userId, int currentUserId)
        {
            await _userBusiness.UserContactEmailRem(userId, currentUserId);
            return Ok();
        }
       
        [HttpPost]
        [Route("ChangePassWord")]
        public async Task<HttpResponseMessage> ChangePassWord(ResetPasswordModel resetPasswordModel)
        {
            var response = await _userBusiness.ChangePassWord(resetPasswordModel);
            return response;
        }
        [HttpPost]
        [Route("PaymentReminder")]
        public async Task<HttpResponseMessage> PaymentReminder(int userId)
        {
            var result = await _userBusiness.PaymentReminder(userId);
            return result;
        }
        [HttpPost]
        [Route("SaveMember")]
        public async Task<IActionResult> SaveMember(int userId,string memberId,int chapterId,int organizationId)
        {
             await _userBusiness.SaveMember(userId, memberId, chapterId,organizationId);
            return Ok();
        }
        [HttpPost]
        [Route("MassPaymentRem")]
        public async Task<HttpResponseMessage> MassPaymentRem([FromBody]List<string> filterMassPaymentModels)
        {
            var response = await _userBusiness.MassPaymentRem(filterMassPaymentModels);
            return response;
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel refreshTokenRequest)
        {
            return null;
        //    // Check if the provided refresh token exists and is valid for the user
        //    var userEntity = await _userBusiness.GetUserByIdAsync(refreshTokenRequest.UserId);
        //    if (userEntity == null || userEntity.RefreshToken != refreshTokenRequest.RefreshToken)
        //    {
        //        return Unauthorized(new { Message = "Invalid refresh token or user ID." });
        //    }

            //    // Optional: Validate expiration of the refresh token if applicable
            //    if (userEntity.TokenExpiryDate <= DateTime.UtcNow)
            //    {
            //        return Unauthorized(new { Message = "Refresh token has expired." });
            //    }

            //    // Map to AuthModel and generate a new access token
            //    var authModel = new AuthModel
            //    {
            //        UserId = userEntity.Id,
            //        Name = userEntity.Name,
            //        Email = userEntity.Email,
            //        UserRoles = userEntity.UserRoles.Select(role => new UserRoleModel
            //        {
            //            Id = role.Id,
            //            RoleName = role.RoleName
            //        }).ToList(),
            //        Organizations = userEntity.UserOrganizations.Select(org => new OrganizationModel
            //        {
            //            OrganizationId = org.OrganizationId,
            //            OrganizationName = org.OrganizationName
            //        }).ToList()
            //    };

            //    // Generate new token
            //    await PopulateJwtTokenAsync(authModel);

            //    // Update user's last login or refresh token expiry if needed
            //    await _userBusiness.UpdatelastLogin(authModel);

            //    // Return new access token
            //    return Ok(new
            //    {
            //        AccessToken = authModel.Token,
            //        ExpiresIn = _appsettings.TokenSettings.SessionExpiryInMinutes * 60
            //    });
        }


    }
}
