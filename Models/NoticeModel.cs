namespace st_xaviers_website_api.Models
{
    public class NoticeModel:BaseModel
    {
        public int? NoticeId { get; set; }
        public string? Notice { get; set; } = "";
        public string? ValidFrom { get; set; }
        public string? ValidTo { get; set; }

    }
}