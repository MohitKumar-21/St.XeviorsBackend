namespace st_xaviers_website_api.Models
{
    public class ContactUsModel : BaseModel
    {
        public int? ContactUsId { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }
    }
}
