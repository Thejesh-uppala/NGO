using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NGO.Business;
using NGO.Data.NGO.Entites;
using NGO.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace NGO.Web.Controllers
{
    [Route("api/v1/user")]
    [ApiController]
    public class UserController : ApiBaseController
    {
        private readonly UserBusiness _userBusiness;
        private readonly EventBusiness _eventBusiness;
        private readonly RegisterBusiness _registerBusiness;

        public UserController(UserBusiness userBusiness, EventBusiness eventBusiness, RegisterBusiness registerBusiness)
        {
            _userBusiness = userBusiness;
            _eventBusiness = eventBusiness;
            _registerBusiness = registerBusiness;
        }

        [SwaggerOperation(
            Summary = "Login api",
            Description = "This API is used to login"
        )]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromQuery] int orgId, LoginModel loginModel)
        {

            var authModel = await _userBusiness.GetUserDetails(orgId, loginModel);
            if (authModel.Error != null)
            {
                return Unauthorized(new { message = authModel.Error });
            }

            return Ok(authModel);

        }

        [SwaggerOperation(
            Summary = "Delete user(s) by UserId(s) and OrgId",
            Description = "This API deletes a user or multiple users based on UserId(s) in the body and OrgId in the query."
        )]
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(
        [FromBody] List<int> userIds,
        [FromQuery] int updatedBy,     // `updatedBy` is still a query parameter
        [FromQuery] int orgId)         // `orgId` remains in the query
        {
            if ((userIds == null || !userIds.Any()) || orgId <= 0)
            {
                return BadRequest(new { Error = "Invalid UserId(s) or OrgId." });
            }

            try
            {
                await _userBusiness.DeleteUsersAsync(userIds, updatedBy, orgId);
                return Ok(new { Message = userIds.Count == 1 ? "User deleted successfully." : "Users deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [SwaggerOperation(
          Summary = "Update user api",
          Description = "This API is used to update a user details"
        )]
        [Authorize]
        [HttpPut]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromQuery] int orgId, [FromForm] string userUpdateModel, [FromForm] IFormFile? photo)
        {
            var userUpdateModelObj = JsonConvert.DeserializeObject<RegistrationModel>(userUpdateModel);
            if (userUpdateModelObj == null)
            {
                return BadRequest("Unable to parse update user model.");
            }

            if (orgId <= 0)
            {
                return BadRequest("Invalid OrgId.");
            }
            try
            {

                var result = await _userBusiness.UpdateUser(orgId, userUpdateModelObj, photo);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }


        [SwaggerOperation(
            Summary = "Accept Memberships",
            Description = "This API is used to Accept Memberships"
        )]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("AcceptMemberships")]
        public async Task<ActionResult> AcceptMemberships(int orgId, [FromBody] ApprovalRequest request)
        {
            var result = await _userBusiness.AcceptMemberships(orgId, request.UserIds, request.UserId);
            if (result)
            {
                return Ok(new { Message = "Accepted Membership successfully" });
            }
            return BadRequest("Failed to Accept Membership.");
        }

        [SwaggerOperation(
        Summary = "Reject Memberships",
        Description = "This API is used to Reject Memberships"
        )]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("RejectMemberships")]
        public async Task<ActionResult> RejectMemberships(int orgId, [FromBody] ApprovalRequest request)
        {
            var result = await _userBusiness.RejectMemberships(orgId, request.UserIds, request.UserId);
            if (result)
            {
                return Ok(new { Message = "Rejected Membership successfully" });
            }
            return BadRequest("Failed to Reject Membership.");
        }

        [SwaggerOperation(
            Summary = "Validate OTP for forgot password",
            Description = "This API validates the OTP sent to the user's email"
        )]
        [AllowAnonymous]
        [HttpPost]
        [Route("ValidateOTP")]
        public async Task<IActionResult> ValidateOTP([FromQuery] int orgId, [FromQuery] string emailId, [FromQuery] string otp)
        {
            try
            {
                if (orgId <= 0 || string.IsNullOrWhiteSpace(emailId) || string.IsNullOrWhiteSpace(otp))
                {
                    return BadRequest(new { Error = "Invalid input parameters." });
                }

                if (!int.TryParse(otp, out int parsedOtp))
                {
                    return BadRequest(new { Error = "OTP must be a valid number." });
                }

                var result = await _userBusiness.ValidateOTP(orgId, emailId, parsedOtp);

                if (result)
                {
                    return Ok(new { Message = "OTP MATCHED" });
                }

                return BadRequest(new { Error = "Invalid OTP" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

        [SwaggerOperation(
           Summary = "send otp (forgot password)",
           Description = "This API is used to snd otp to mail id"
        )]
        [AllowAnonymous]
        [HttpPost]
        [Route("SendOTP")]
        public async Task<HttpResponseMessage> SendOTPData(string emailId, [FromQuery] int orgId)
        {
            var response = await _userBusiness.ForgotPasswordSendOTP(emailId, orgId);
            return response;
        }

        [SwaggerOperation(
           Summary = "get all user api",
           Description = "gets all users"
        )]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetAllUser")]
        public async Task<IActionResult> GetAllUserProfiles([FromQuery] int orgId)
        {
            var userslist = await _userBusiness.GetAllUserProfiles(orgId);
            return new JsonResult(new { userslist });
        }

        [SwaggerOperation(
          Summary = "get all Members api",
                        Description = "gets all users"
        )]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetAllMembers")]
        public async Task<IActionResult> GetAllMembersProfiles([FromQuery] int orgId)
        {
            var userslist = await _userBusiness.GetAllMembersProfiles(orgId);
            return new JsonResult(new { userslist });
        }

        [SwaggerOperation(
           Summary = "brand details fetch",
           Description = "This API is used to fetch brand details"
        )]
        [AllowAnonymous]
        [HttpGet("{orgId}/brand-details")]
        public async Task<IActionResult> GetBrandDetails(int orgId)
        {
            try
            {
                var (brandImageUrl, brandColor, orgName, orgWelMsg, brandLogo, primaryEmail, matrimonyEmail, eventEmail, orgHeadEmail) =
                          await _userBusiness.GetBrandDetailsAsync(orgId);
                return Ok(new { BrandImage = brandImageUrl, BrandColor = brandColor, OrgName = orgName, OrgWelMsg = orgWelMsg, BrandLogo = brandLogo });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [SwaggerOperation(
           Summary = "get all organization",
           Description = "get all organization"
        )]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetAllOrganizations")]
        public async Task<IActionResult> GetAllOrganizations()
        {
            var response = await _userBusiness.GetAllOrganizations();
            return Ok(response);
        }

        [SwaggerOperation(
           Summary = "Get CurrentUserProfile api",
           Description = "This API is used to  GetCurrentUserProfile"
        )]
        [Authorize]
        [HttpGet]
        [Route("GetCurrentUserProfile")]
        public async Task<IActionResult> GetCurrentUserProfile([FromQuery] int orgId, int userId)
        {
            var userData = await _userBusiness.GetCurrentUserProfile(orgId, userId);
            return new JsonResult(new { userData });
        }


        [SwaggerOperation(
         Summary = "Get Current user custom data api",
         Description = "This API is used to  GetCurrentUserProfileCustomData"
        )]
        [Authorize]
        [HttpGet]
        [Route("GetCurrentUserCustomData")]
        public async Task<IActionResult> GetCurrentUserCustomData([FromQuery] int orgId, int userId)
        {
            var userDetails = await _userBusiness.GetCurrentUserCustomData(orgId, userId);
            var userData = new UserBasicInfo
            {
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                Photo = userDetails.Photo,
                PhoneNumber = userDetails.PhoneNumber,
                YearOfBirth = userDetails.YearOfBirth,
                EmailAddress = userDetails.UniqueId
            };


            return new JsonResult(new { userData });
        }

        [SwaggerOperation(
            Summary = "Get summary counts",
            Description = "Provides counts for total users, members, and upcoming events"
        )]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetSummaryCounts")]
        public async Task<IActionResult> GetSummaryCounts([FromQuery] int orgId)
        {
            try
            {

                var totalUsers = await _userBusiness.GetUserCount(orgId);
                var totalMembers = await _userBusiness.GetMemberCount(orgId);
                var upcomingEvents = await _eventBusiness.GetUpcomingEventsAsync(orgId);

                var result = new
                {
                    TotalUsers = totalUsers,
                    TotalMembers = totalMembers,
                    UpcomingEvents = upcomingEvents.Count()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }


        [SwaggerOperation(
           Summary = "forgot password  api",
           Description = "This API is used when the user not knows his password"
        )]
        [AllowAnonymous]
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromQuery] int orgId, [FromQuery] string emailId, [FromQuery] string newPassword)
        {
            try
            {
                // Validate input parameters
                if (orgId <= 0 || string.IsNullOrWhiteSpace(emailId) || string.IsNullOrWhiteSpace(newPassword))
                {
                    return BadRequest(new { Error = "Invalid input parameters." });
                }

                // Call business logic
                var result = await _userBusiness.ForgotPassword(orgId, emailId, newPassword);

                if (!result.IsSuccess)
                {
                    return StatusCode(result.StatusCode, new { Error = result.Error });
                }

                return Ok(new { Message = result.Message });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }


        [SwaggerOperation(
           Summary = "change password api",
           Description = "This API is used to change password"
        )]
        [Authorize]
        [HttpPost]
        [Route("ChangePassWord")]
        public async Task<IActionResult> ChangePassword(int orgId, [FromBody] ResetPasswordModel model)
        {
            var result = await _userBusiness.ChangePasswordAsync(orgId, model);

            if (!result.IsSuccess)
            {
                var errorMessage = result.Error == "Incorrect Password" ? "incorrect password" : result.Error;
                return StatusCode(result.StatusCode, new { Error = errorMessage });
            }

            return Ok(new { Message = "Password updated successfully." });
        }

        [SwaggerOperation(
           Summary = "getallchapters",
           Description = "This API is used to getall chapters"
        )]
        [AllowAnonymous]
        [HttpGet]
        [Route("GetOrgChapters")]
        public async Task<List<DropDownModel>> GetOrgChapters([FromQuery] int orgId)
        {
            var response = await _userBusiness.GetAllChapters(orgId);
            return response;
        }

        [SwaggerOperation(
           Summary = "get all Membership types",
           Description = "This API is used to getall Membership types"
        )]
        [Authorize]
        [HttpGet]
        [Route("GetOrgMembershipTypes")]
        public async Task<List<GetMemberShipTypeDropdown>> GetOrgMembershipData([FromQuery] int orgId)
        {
            var response = await _userBusiness.GetOrgMembershipData(orgId);
            return response;
        }

    }
}
