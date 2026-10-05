using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using st_xaviers_website_api.Dal;
using st_xaviers_website_api.Models;
using st_xaviers_website_api.Services;

namespace st_xaviers_website_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopperController : ControllerBase
    {
        private readonly TopperDal _topperDal;
        private readonly CloudinaryService _cloudinaryService;

        public TopperController(TopperDal topperDal, CloudinaryService cloudinaryService)
        {
            _topperDal = topperDal;
            _cloudinaryService = cloudinaryService;
        }

        // Token se logged-in user ka RoleId nikalta hai (na mile to 0)
        private int GetRoleId()
        {
            var claim = User.FindFirst("RoleId")?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }

        [Authorize]
        [HttpPost("Insert")]
        public IActionResult Insert([FromForm] TopperFormDto dto)
        {
            try
            {
                var model = new TopperModel
                {
                    Name = dto.Name,
                    Class = dto.Class,
                    ResultYear = dto.ResultYear,
                    Percentage = dto.Percentage,
                    PhotoPath = _cloudinaryService.UploadPhoto(dto.Photo, "topper-photos"),
                    AddedBy = GetRoleId()
                };

                int id = _topperDal.Insert(model);
                return Ok(new { Success = true, Message = "Topper inserted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("Update")]
        public IActionResult Update([FromForm] TopperFormDto dto)
        {
            try
            {
                var model = new TopperModel
                {
                    TopperId = dto.TopperId,
                    Name = dto.Name,
                    Class = dto.Class,
                    ResultYear = dto.ResultYear,
                    Percentage = dto.Percentage,
                    PhotoPath = _cloudinaryService.UploadPhoto(dto.Photo, "topper-photos"),
                    UpdatedBy = GetRoleId()
                };

                bool result = _topperDal.Update(model);
                if (!result) return NotFound(new { Success = false, Message = "Topper not found." });

                return Ok(new { Success = true, Message = "Topper updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("Delete/{topperId}")]
        public IActionResult Delete(int topperId)
        {
            try
            {
                bool result = _topperDal.Delete(topperId, GetRoleId());
                if (!result) return NotFound(new { Success = false, Message = "Topper not found." });

                return Ok(new { Success = true, Message = "Topper deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // ADMIN: sab toppers
        [HttpGet("Get")]
        public IActionResult Get()
        {
            try
            {
                var result = _topperDal.Get();
                return Ok(new { Success = true, Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // WEBSITE (public): current year ke toppers, optional class filter
        [AllowAnonymous]
        [HttpGet("GetCurrentYear")]
        public IActionResult GetCurrentYear([FromQuery] string? studentClass)
        {
            try
            {
                var result = _topperDal.GetCurrentYear(studentClass);
                return Ok(new { Success = true, Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetById/{topperId}")]
        public IActionResult GetById(int topperId)
        {
            try
            {
                var result = _topperDal.Get(topperId);
                if (result.Count == 0) return NotFound(new { Success = false, Message = "Topper not found." });

                return Ok(new { Success = true, Data = result[0] });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}