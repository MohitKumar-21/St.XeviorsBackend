namespace st_xaviers_website_api.Models
{
    public class TopperModel:BaseModel
    {
        public int? TopperId { get; set; }
        public string? Name { get; set; }
        public string? Class { get; set; }
        public string? ResultYear { get; set; }
        public decimal? Percentage { get; set; }
        public string? PhotoPath { get; set; }
    }

    public class TopperFormDto
    {
        public int TopperId { get; set; }
        public string? Name { get; set; }
        public string? Class { get; set; }
        public string? ResultYear { get; set; }
        public decimal? Percentage { get; set; }
        public IFormFile? Photo { get; set; }
    }
}