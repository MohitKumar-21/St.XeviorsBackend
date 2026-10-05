using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using st_xaviers_website_api.Dal;
using st_xaviers_website_api.Models;

namespace st_xaviers_website_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnquiryFormController : ControllerBase
    {
        private readonly EnquiryFormDal _enquiryFormDal;

        public EnquiryFormController(EnquiryFormDal enquiryFormDal)
        {
            _enquiryFormDal = enquiryFormDal;
        }

        // Token se logged-in user ka RoleId nikalta hai (na mile to 0)
        private int GetRoleId()
        {
            var claim = User.FindFirst("RoleId")?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }

        // INSERT (public form: bina login AddedBy = 0)
        [HttpPost("Insert")]
        public IActionResult Insert([FromBody] EnquiryFormModel model)
        {
            try
            {
                // AddedBy AuditFilter ne bhar diya
                int id = _enquiryFormDal.Insert(model);

                return Ok(new
                {
                    Success = true,
                    Message = "Enquiry submitted successfully.",
                    //Id = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // UPDATE
        [Authorize]
        [HttpPut("Update")]
        public IActionResult Update([FromBody] EnquiryFormModel model)
        {
            try
            {
                // UpdatedBy AuditFilter ne bhar diya
                bool result = _enquiryFormDal.Update(model);

                if (!result)
                {
                    return NotFound(new { Success = false, Message = "Enquiry not found." });
                }

                return Ok(new { Success = true, Message = "Enquiry updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // DELETE
        [Authorize]
        [HttpDelete("Delete/{EnquiryFormId}")]
        public IActionResult Delete(int EnquiryFormId)
        {
            try
            {
                bool result = _enquiryFormDal.Delete(EnquiryFormId, GetRoleId());

                if (!result)
                {
                    return NotFound(new { Success = false, Message = "Enquiry not found." });
                }

                return Ok(new { Success = true, Message = "Enquiry deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET ALL
        [Authorize]
        [HttpGet("Get")]
        public IActionResult Get()
        {
            try
            {
                List<EnquiryFormModel> result = _enquiryFormDal.Get();

                return Ok(new { Success = true, Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET BY ID
        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                List<EnquiryFormModel> result = _enquiryFormDal.Get(id);

                if (result == null || result.Count == 0)
                {
                    return NotFound(new { Success = false, Message = "Enquiry not found." });
                }

                return Ok(new { Success = true, Data = result[0] });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}