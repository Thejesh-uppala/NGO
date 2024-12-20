﻿using System.Diagnostics;
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
        public async Task<IActionResult> Login([FromQuery] int orgId,LoginModel loginModel)
        {



            var authModel = await _userBusiness.GetUserDetails(orgId, loginModel);
            if (authModel.Error != null)
            {
                return Unauthorized(new { message = authModel.Error });
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
        [AllowAnonymous]
        [HttpPost]
        [Route("ChangePassWord")]
        public async Task<IActionResult> ChangePassword(int orgId, [FromBody] ResetPasswordModel model)
        {
            var result = await _userBusiness.ChangePasswordAsync(orgId, model);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { Error = result.Error });
            }

            return Ok(new { Message = "Password updated successfully." });
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

        [AllowAnonymous]
        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestModel request)
        {
            var authModel = await _userBusiness.GenerateNewAccessTokenAsync(request.UserId, request.RefreshToken);

            if (authModel == null)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token." });
            }

            return Ok(authModel);
        }




    }
}
