using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using st_xaviers_website_api.DAL;
using st_xaviers_website_api.Models;

namespace st_xaviers_website_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private readonly ContactUsDal _contactUsDal;

        public ContactUsController(ContactUsDal contactUsDal)
        {
            _contactUsDal = contactUsDal;
        }

        // Token se logged-in user ka RoleId nikalta hai (na mile to 0)
        private int GetRoleId()
        {
            var claim = User.FindFirst("RoleId")?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }


        // INSERT (public form: bina login ke AddedBy = 0, login ho to RoleId)
        [HttpPost("Insert")]
        public IActionResult Insert(ContactUsModel model)
        {
            try
            {
                // AddedBy AuditFilter ne bhar diya
                int id = _contactUsDal.Insert(model);

                return Ok(new
                {
                    Success = true,
                    Message = "Contact message inserted successfully.",
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
        public IActionResult Update(ContactUsModel model)
        {
            try
            {
                // UpdatedBy AuditFilter ne bhar diya
                bool result = _contactUsDal.Update(model);

                if (!result)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Contact message not found."
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Contact message updated successfully."
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
        [HttpDelete("Delete/{ContactUsId}")]
        public IActionResult Delete(int ContactUsId)
        {
            try
            {
                bool result = _contactUsDal.Delete(ContactUsId, GetRoleId());

                if (!result)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Contact message not found."
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Contact message deleted successfully."
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
        [Authorize]
        [HttpGet("Get")]
        public IActionResult Get()
        {
            try
            {
                List<ContactUsModel> result = _contactUsDal.Get();

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
        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                List<ContactUsModel> result = _contactUsDal.Get(id);

                if (result.Count == 0)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Contact message not found."
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