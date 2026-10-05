using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using st_xaviers_website_api.Dal;
using st_xaviers_website_api.DAL;
using st_xaviers_website_api.Models;
using System;
using System.Globalization;
using System.Linq;

namespace st_xaviers_website_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticeController : ControllerBase
    {
        private readonly NoticeDal _noticeDal;

        private static readonly string[] DateFormats = { "dd-MM-yyyy", "dd/MM/yyyy", "dd.MM.yyyy", "yyyy-MM-dd" };

        public NoticeController(NoticeDal noticeDal)
        {
            _noticeDal = noticeDal;
        }

        private int GetRoleId()
        {
            var claim = User.FindFirst("RoleId")?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }


        // ValidFrom aur ValidTo ko "yyyy-MM-dd" mein badalta hai.
        // Galat ho to error message deta hai, sahi ho to null.
        private static string? TryNormalizeDates(NoticeModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ValidFrom) || string.IsNullOrWhiteSpace(model.ValidTo))
                return "Valid From aur Valid To zaroori hai.";

            if (!DateTime.TryParseExact(model.ValidFrom.Trim(), DateFormats,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime from))
                return "Invalid ValidFrom. Use dd-MM-yyyy.";

            if (!DateTime.TryParseExact(model.ValidTo.Trim(), DateFormats,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime to))
                return "Invalid ValidTo. Use dd-MM-yyyy.";

            if (to < from)
                return "Valid To, Valid From se pehle nahi ho sakta.";

            model.ValidFrom = from.ToString("yyyy-MM-dd");
            model.ValidTo = to.ToString("yyyy-MM-dd");
            return null;
        }

        // Token se logged-in admin ki UserId nikalta hai
        private int? GetUserId()
        {
            var claim = User.FindFirst("UserId")?.Value;
            return int.TryParse(claim, out int id) ? id : null;
        }

        // INSERT
        [Authorize]
        [HttpPost("Insert")]
        public IActionResult Insert([FromBody] NoticeModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Notice))
                return BadRequest(new { Success = false, Message = "Notice zaroori hai." });

            string? error = TryNormalizeDates(model);
            if (error != null)
                return BadRequest(new { Success = false, Message = error });

            try
            {
                int id = _noticeDal.Insert(model);   // AddedBy filter ne bhar diya

                return Ok(new { Success = true, Message = "Notice saved successfully.", NoticeId = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // UPDATE
        [Authorize]
        [HttpPut("Update")]
        public IActionResult Update([FromBody] NoticeModel model)
        {
            if (model.NoticeId == null)
                return BadRequest(new { Success = false, Message = "NoticeId zaroori hai." });

            if (string.IsNullOrWhiteSpace(model.Notice))
                return BadRequest(new { Success = false, Message = "Notice zaroori hai." });

            string? error = TryNormalizeDates(model);
            if (error != null)
                return BadRequest(new { Success = false, Message = error });

            try
            {
                // UpdatedBy filter ne bhar diya
                if (!_noticeDal.Update(model))
                    return NotFound(new { Success = false, Message = "Notice not found." });

                return Ok(new { Success = true, Message = "Notice updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // DELETE
        [Authorize]
        [HttpDelete("Delete/{NoticeId}")]
        public IActionResult Delete(int NoticeId)
        {
            try
            {
                if (!_noticeDal.Delete(NoticeId, GetRoleId()))
                    return NotFound(new { Success = false, Message = "Notice not found." });

                return Ok(new { Success = true, Message = "Notice deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET ALL (dashboard)
        [HttpGet("Get")]
        public IActionResult Get()
        {
            try
            {
                return Ok(_noticeDal.Get());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET BY ID (Update modal prefill)
        [Authorize]
        [HttpGet("GetById/{NoticeId}")]
        public IActionResult GetById(int NoticeId)
        {
            try
            {
                var item = _noticeDal.Get(NoticeId).FirstOrDefault();
                if (item == null)
                    return NotFound(new { Success = false, Message = "Notice not found." });

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET ACTIVE (website, public)
        [AllowAnonymous]
        [HttpGet("GetActive")]
        public IActionResult GetActive()
        {
            try
            {
                return Ok(_noticeDal.GetActive());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}