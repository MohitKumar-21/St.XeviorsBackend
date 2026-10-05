// Controllers/GalleryController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using st_xaviers_website_api.DAL;
using st_xaviers_website_api.Models;
using st_xaviers_website_api.Services;

namespace st_xaviers_website_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GalleryController : ControllerBase
    {
        private const long MaxFileSize = 5 * 1024 * 1024;   // 5 MB per image
        private const int MaxFiles = 20;                    // ek baar me max 20 images

        private readonly GalleryDal _dal;
        private readonly CloudinaryService _cloudinary;

        public GalleryController(GalleryDal dal, CloudinaryService cloudinary)
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

        // MULTIPLE IMAGES UPLOAD
        [Authorize]
        [HttpPost("Insert")]
        [RequestSizeLimit(120_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 120_000_000)]
        public IActionResult Insert([FromForm] GalleryFormDto dto)
        {
            try
            {
                if (dto.CoverImgId <= 0)
                    return BadRequest(new { Success = false, Message = "Gallery category is required." });

                if (dto.Images == null || dto.Images.Count == 0)
                    return BadRequest(new { Success = false, Message = "Please select at least one image." });

                if (dto.Images.Count > MaxFiles)
                    return BadRequest(new { Success = false, Message = $"Maximum {MaxFiles} images allowed at a time." });

                foreach (var file in dto.Images)
                {
                    if (file.Length == 0 || !file.ContentType.StartsWith("image/"))
                        return BadRequest(new { Success = false, Message = $"'{file.FileName}' is not a valid image." });

                    if (file.Length > MaxFileSize)
                        return BadRequest(new { Success = false, Message = $"'{file.FileName}' is larger than 5 MB." });
                }

                var paths = new List<string>();
                foreach (var file in dto.Images)
                {
                    var url = _cloudinary.UploadPhoto(file, "gallery-images");
                    if (!string.IsNullOrEmpty(url)) paths.Add(url);
                }

                if (paths.Count == 0)
                    return StatusCode(500, new { Success = false, Message = "Image upload failed." });

                int saved = _dal.InsertMany(dto.CoverImgId, paths, GetRoleId());

                return Ok(new
                {
                    Success = true,
                    Message = $"{saved} image(s) uploaded successfully.",
                    Count = saved
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET ALL (ya category ke hisaab se: /api/Gallery/Get?coverImgId=1)
        [HttpGet("Get")]
        public IActionResult Get([FromQuery] int? coverImgId)
        {
            try
            {
                return Ok(new { Success = true, Data = _dal.Get(coverImgId) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // UPDATE (single image replace / category change)
        [Authorize]
        [HttpPut("Update")]
        public IActionResult Update([FromForm] GalleryUpdateDto dto)
        {
            try
            {
                if (dto.GalleryId <= 0)
                    return BadRequest(new { Success = false, Message = "GalleryId is required." });

                if (dto.Image == null && (dto.CoverImgId == null || dto.CoverImgId <= 0))
                    return BadRequest(new { Success = false, Message = "Nothing to update. Send a new image or a category." });

                string? newPath = null;
                if (dto.Image != null)
                {
                    if (dto.Image.Length == 0 || !dto.Image.ContentType.StartsWith("image/"))
                        return BadRequest(new { Success = false, Message = "Invalid image file." });

                    if (dto.Image.Length > MaxFileSize)
                        return BadRequest(new { Success = false, Message = "Image is larger than 5 MB." });

                    newPath = _cloudinary.UploadPhoto(dto.Image, "gallery-images");
                }

                int? categoryId = dto.CoverImgId > 0 ? dto.CoverImgId : null;

                if (!_dal.Update(dto.GalleryId, categoryId, newPath, GetRoleId()))
                    return NotFound(new { Success = false, Message = "Gallery image not found." });

                return Ok(new { Success = true, Message = "Gallery image updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // DELETE (soft delete, single image)
        [Authorize]
        [HttpDelete("Delete/{GalleryId}")]
        public IActionResult Delete(int GalleryId)
        {
            try
            {
                if (!_dal.Delete(GalleryId, GetRoleId()))
                    return NotFound(new { Success = false, Message = "Image not found." });

                return Ok(new { Success = true, Message = "Image deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}