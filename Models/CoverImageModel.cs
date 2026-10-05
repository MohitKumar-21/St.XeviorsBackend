// Models/CoverImageModel.cs
namespace st_xaviers_website_api.Models
{
    public class CoverImageModel:BaseModel
    {
        public int? CoverImgId { get; set; }
        public string? CoverTitle { get; set; }
        public string? ImagePath { get; set; }
 
    }

    public class CoverImageFormDto
    {
        public int CoverImgId { get; set; }          // Update me chahiye
        public string CoverTitle { get; set; } = "";
        public IFormFile? CoverImage { get; set; }   // form ka file input
    }
}