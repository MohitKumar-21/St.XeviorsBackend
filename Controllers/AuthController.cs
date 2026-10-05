using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using st_xaviers_website_api.Dal;
using st_xaviers_website_api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserDal _userDal;
    private readonly IConfiguration _config;

    public AuthController(UserDal userDal, IConfiguration config)
    {
        _userDal = userDal;
        _config = config;
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { Success = false, Message = "Username aur Password zaroori hai." });

            UserModel? user = _userDal.Login(request.Username.Trim(), request.Password);

            if (user == null)
                return Unauthorized(new { Success = false, Message = "Username ya Password galat hai." });

            _userDal.UpdateLastLogin(user.UserId);

            var expiry = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiryMinutes"]));

            var claims = new[]
            {
                new Claim("UserId", user.UserId.ToString()),
                new Claim("RoleId", user.RoleId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.RoleId == 1 ? "Admin" : "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds);

            return Ok(new
            {
                Success = true,
                Message = "Login successful.",
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresOn = expiry,
                User = new { user.UserId, user.Username, user.FullName, user.RoleId }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Success = false, Message = ex.Message });
        }
    }
}