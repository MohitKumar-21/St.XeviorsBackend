// Controllers/CoverImageController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using st_xaviers_website_api.DAL;
using st_xaviers_website_api.Models;
using st_xaviers_website_api.Services;

namespace st_xaviers_website_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoverImageController : ControllerBase
    {
        private readonly CoverImageDal _dal;
        private readonly CloudinaryService _cloudinary;

        public CoverImageController(CoverImageDal dal, CloudinaryService cloudinary)
        {
            _dal = dal;
            _cloudinary = cloudinary;
        }

        // Token se logged-in user ka RoleId nikalta hai (na mile to 0)
        private int GetRoleId()
        {
            var claim = User.FindFirst("RoleId")?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }

        // INSERT
        [Authorize]
        [HttpPost("Insert")]
        public IActionResult Insert([FromForm] CoverImageFormDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.CoverTitle))
                    return BadRequest(new { Success = false, Message = "Category name is required." });

                if (dto.CoverImage == null || dto.CoverImage.Length == 0)
                    return BadRequest(new { Success = false, Message = "Cover image is required." });

                var model = new CoverImageModel
                {
                    CoverTitle = dto.CoverTitle.Trim(),
                    ImagePath = _cloudinary.UploadPhoto(dto.CoverImage, "cover-images"),
                    AddedBy = GetRoleId()
                };

                int coverImgId = _dal.Insert(model);

                return Ok(new { Success = true, Message = "Cover image added successfully.", CoverImgId = coverImgId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // UPDATE (image optional - na bheji to purani rahegi)
        [Authorize]
        [HttpPut("Update")]
        public IActionResult Update([FromForm] CoverImageFormDto dto)
        {
            try
            {
                if (dto.CoverImgId <= 0)
                    return BadRequest(new { Success = false, Message = "CoverImgId is required." });

                var model = new CoverImageModel
                {
                    CoverImgId = dto.CoverImgId,
                    CoverTitle = dto.CoverTitle?.Trim(),
                    ImagePath = dto.CoverImage != null
                        ? _cloudinary.UploadPhoto(dto.CoverImage, "cover-images")
                        : null,
                    UpdatedBy = GetRoleId()
                };

                if (!_dal.Update(model))
                    return NotFound(new { Success = false, Message = "Cover image not found." });

                return Ok(new { Success = true, Message = "Cover image updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // DELETE (soft delete)
        [Authorize]
        [HttpDelete("Delete/{coverImgId}")]
        public IActionResult Delete(int coverImgId)
        {
            try
            {
                if (!_dal.Delete(coverImgId, GetRoleId()))
                    return NotFound(new { Success = false, Message = "Cover image not found." });

                return Ok(new { Success = true, Message = "Cover image deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET ALL (website pe dikhane ke liye public rakha hai, chaho to [Authorize] laga do)
        [HttpGet("Get")]
        public IActionResult Get()
        {
            try
            {
                return Ok(new { Success = true, Data = _dal.Get() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET BY ID
        [HttpGet("GetById/{coverImgId}")]
        public IActionResult GetById(int coverImgId)
        {
            try
            {
                var result = _dal.Get(coverImgId);
                if (result.Count == 0)
                    return NotFound(new { Success = false, Message = "Cover image not found." });

                return Ok(new { Success = true, Data = result[0] });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}