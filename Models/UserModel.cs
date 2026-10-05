namespace st_xaviers_website_api.Models
{

    public class UserModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string? FullName { get; set; }
        public int RoleId { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    
}
