using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using st_xaviers_website_api.DAL;
using st_xaviers_website_api.Models;
using st_xaviers_website_api.Services;

namespace st_xaviers_website_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestimonialController : ControllerBase
    {
        private readonly TestimonialDal _testimonialDal;
        private readonly CloudinaryService _cloudinaryService;

        public TestimonialController(TestimonialDal testimonialDal, CloudinaryService cloudinaryService)
        {
            _testimonialDal = testimonialDal;
            _cloudinaryService = cloudinaryService;
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
        public IActionResult Insert([FromForm] TestimonialFormDto dto)
        {
            try
            {
                var model = new TestimonialModel
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    PhotoPath = _cloudinaryService.UploadPhoto(dto.Photo, "testimonial-photos"),
                    AddedBy = GetRoleId()
                };

                int id = _testimonialDal.Insert(model);

                return Ok(new
                {
                    Success = true,
                    Message = "Testimonial inserted successfully.",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        // UPDATE
        [Authorize]
        [HttpPut("Update")]
        public IActionResult Update([FromForm] TestimonialFormDto dto)
        {
            try
            {
                var model = new TestimonialModel
                {
                    TestimonialId = dto.TestimonialId,
                    Name = dto.Name,
                    Description = dto.Description,
                    PhotoPath = _cloudinaryService.UploadPhoto(dto.Photo, "testimonial-photos"),
                    UpdatedBy = GetRoleId()
                };

                bool result = _testimonialDal.Update(model);

                if (!result)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Testimonial not found."
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Testimonial updated successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        // DELETE
        [Authorize]
        [HttpDelete("Delete/{testimonialId}")]
        public IActionResult Delete(int testimonialId)
        {
            try
            {
                bool result = _testimonialDal.Delete(testimonialId, GetRoleId());

                if (!result)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Testimonial not found."
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Testimonial deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        // GET ALL
        [HttpGet("Get")]
        public IActionResult Get()
        {
            try
            {
                List<TestimonialModel> result = _testimonialDal.Get();

                return Ok(new
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        // GET BY ID
        [Authorize]
        [HttpGet("GetById/{testimonialId}")]
        public IActionResult GetById(int testimonialId)
        {
            try
            {
                List<TestimonialModel> result = _testimonialDal.Get(testimonialId);

                if (result.Count == 0)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Testimonial not found."
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Data = result[0]
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}