using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NGO.Business;
using NGO.Data;
using NGO.Data.NGO.Entites;
using NGO.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace NGO.Web.Controllers
{
    [Route("api/v1/matrimonial")]
    [ApiController]
    public class MatrimonialController : ApiBaseController
    {
        private readonly MatrimonialBusiness _matrimonialBusiness;

        public MatrimonialController(MatrimonialBusiness matrimonialBusiness)
        {
            _matrimonialBusiness = matrimonialBusiness;
        }

        [SwaggerOperation(
         Summary = "Get Matrimonial Data By Id",
         Description = "This API is used to Get MatrimonialData By Id"
      )]
        [Authorize]
        [HttpGet]
        [Route("GetMatrimonialDataById")]
        public async Task<ActionResult<MatrimonialDataResponse>> GetMatrimonialDataById(int Id, int orgId)
        {
            var data = await _matrimonialBusiness.GetMatrimonialDataByIdAsync(Id, orgId);
            if (data == null)
            {
                return NotFound("Not Found");
            }
            return data;
        }


        [SwaggerOperation(
         Summary = "GetAllApprovedMatrimonialData api",
         Description = "This API is used to Get All MatrimonialData"
        )]
        [Authorize]
        [HttpGet]
        [Route("GetAllApprovedMatrimonialData")]
        public async Task<ActionResult<IEnumerable<MatrimonialData>>> GetAllApprovedMatrimonialData([FromQuery] int orgId, [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
        {
            var (matrimonialUsersList, totalRecords) = await _matrimonialBusiness.GetAllApprovedMatrimonialData(orgId, page, pageSize);

            return new JsonResult(new
            {
                TotalRecords = totalRecords,
                Page = page,
                PageSize = pageSize,
                matrimonialuserslist = matrimonialUsersList
            });
        }


        [SwaggerOperation(
         Summary = "GetAllMatrimonialData api",
         Description = "This API is used to Get All MatrimonialData"
      )]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetAllMatrimonialData")]
        public async Task<ActionResult<IEnumerable<MatrimonialData>>> GetAllMatrimonialData([FromQuery] int orgId)
        {
            var matrimonialuserslist = await _matrimonialBusiness.GetAllMatrimonialDataAsync(orgId);
            return new JsonResult(new { matrimonialuserslist });
        }


        [SwaggerOperation(
        Summary = "Get Filtered MatrimonialData api",
        Description = "This API is used to Get Filtered MatrimonialData"
     )]
        [Authorize]
        [HttpGet]
        [Route("GetFilteredMatrimonialData")]
        public async Task<ActionResult<IEnumerable<MatrimonialData>>> GetFilteredMatrimonialData(
          [FromQuery] int orgId,
          [FromQuery] string? gender,
          [FromQuery] int? minAge,
          [FromQuery] int? maxAge,
          [FromQuery] string? minHeight,
          [FromQuery] string? maxHeight,
          [FromQuery] string? state,
          [FromQuery] string? country,
          [FromQuery] string? education,
          [FromQuery] string? profession,
          [FromQuery] string? prevMarried,
            [FromQuery] string? searchValue,
           [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
        {
            var (filteredData, totalRecords) = await _matrimonialBusiness.GetFilteredMatrimonialDataAsync(orgId, gender, minAge, maxAge, minHeight, maxHeight, state, country, education, profession, prevMarried, searchValue, page, pageSize);
            return new JsonResult(new
            {
                TotalRecords = totalRecords,
                Page = page,
                PageSize = pageSize,
                matrimonialuserslist = filteredData
            });
        }


        [SwaggerOperation(
         Summary = "Create MatrimonialData api",
         Description = "This API is used to CreateMatrimonialData"
      )]

        [Authorize]
        [HttpPost]
        [Route("CreateMatrimonialData")]
        public async Task<ActionResult> CreateMatrimonialData([FromForm] MatrimonialDTO data)
        {
            var id = await _matrimonialBusiness.CreateMatrimonialDataAsync(data);
            return CreatedAtAction(nameof(GetMatrimonialDataById), new { id = id }, data);
        }

        [SwaggerOperation(
         Summary = "Approve Matrimonial Profiles api",
         Description = "This API is used Approve Matrimonial Profiles"
      )]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("ApproveProfiles")]
        public async Task<ActionResult> ApproveProfiles(int orgId, [FromBody] ApprovalRequest request)
        {
            var result = await _matrimonialBusiness.UpdateProfileStatusAsync(orgId, request.UserIds, request.UserId, true);

            //var result = await _matrimonialBusiness.ApproveProfilesAsync(orgId, request.UserIds, request.UserId);
            if (result)
            {
                return Ok(new { Message = "Profiles approved successfully" });
            }
            return BadRequest("Failed to approve profiles.");
        }

        [SwaggerOperation(
         Summary = "Reject Matrimonial Profiles",
         Description = "This API is used to Reject Matrimonial Profiles"
      )]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("RejectProfiles")]
        public async Task<ActionResult> RejectProfiles(int orgId, [FromBody] ApprovalRequest request)
        {
            var result = await _matrimonialBusiness.UpdateProfileStatusAsync(orgId, request.UserIds, request.UserId, false);

            //var result = await _matrimonialBusiness.RejectProfilesAsync(orgId, request.UserIds, request.UserId);
            if (result)
            {
                return Ok(new { Message = "Profiles rejected successfully" });
            }
            return BadRequest("Failed to reject profiles.");
        }






    }
}


