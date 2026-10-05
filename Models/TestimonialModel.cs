namespace st_xaviers_website_api.Models
{
    public class TestimonialModel: BaseModel
    {
        public int? TestimonialId { get; set; }
        public string? Name { get; set; }
        public string? PhotoPath { get; set; }
        public string? Description { get; set; }
    }
    public class TestimonialFormDto
    {
        public int TestimonialId { get; set; }
        public string? Name { get; set; }   
        public string? Description { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
