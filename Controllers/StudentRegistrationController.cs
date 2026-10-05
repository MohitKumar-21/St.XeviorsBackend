using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using st_xaviers_website_api.DAL;
using st_xaviers_website_api.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace st_xaviers_website_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentRegistrationController : ControllerBase
    {
        private readonly StudentRegistrationDal _registrationDal;

        private static readonly string[] DateFormats = { "dd-MM-yyyy", "dd/MM/yyyy", "dd.MM.yyyy", "yyyy-MM-dd" };

        public StudentRegistrationController(StudentRegistrationDal registrationDal)
        {
            _registrationDal = registrationDal;
        }

        // Token se logged-in user ka RoleId nikalta hai (na mile to 0)
        private int GetRoleId()
        {
            var claim = User.FindFirst("RoleId")?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }

        // DateOfBirth ko "yyyy-MM-dd" mein badalta hai. Galat date par false return karta hai.
        private static bool TryNormalizeDob(StudentRegistrationModel model)
        {
            if (string.IsNullOrWhiteSpace(model.DateOfBirth))
                return false;

            if (!DateTime.TryParseExact(model.DateOfBirth.Trim(), DateFormats,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dob))
                return false;

            model.DateOfBirth = dob.ToString("yyyy-MM-dd");
            return true;
        }

        // INSERT (public form: bina login AddedBy = 0)
        [HttpPost("Insert")]
        public IActionResult Insert(StudentRegistrationModel model)
        {
            if (!TryNormalizeDob(model))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Invalid DateOfBirth. Use dd-MM-yyyy."
                });
            }

            try
            {
                // AddedBy AuditFilter ne bhar diya
                int id = _registrationDal.Insert(model);

                return Ok(new
                {
                    Success = true,
                    Message = "Registration completed successfully.",
                    //Id = id
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
        public IActionResult Update(StudentRegistrationModel model)
        {
            if (!TryNormalizeDob(model))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Invalid DateOfBirth. Use dd-MM-yyyy."
                });
            }

            try
            {
                // UpdatedBy AuditFilter ne bhar diya
                bool result = _registrationDal.Update(model);

                if (!result)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Registration not found."
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Registration updated successfully."
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
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                bool result = _registrationDal.Delete(id, GetRoleId());

                if (!result)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Registration not found."
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Registration deleted successfully."
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
                List<StudentRegistrationModel> result = _registrationDal.Get();

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
                List<StudentRegistrationModel> result = _registrationDal.Get(id);

                if (result == null || result.Count == 0)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Registration not found."
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