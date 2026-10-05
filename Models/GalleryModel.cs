// Models/GalleryModel.cs
namespace st_xaviers_website_api.Models
{
    public class GalleryModel:BaseModel
    {
        public int? GalleryId { get; set; }
        public int? CoverImgId { get; set; }
        public string? CoverTitle { get; set; }
        public string? ImagePath { get; set; }
    }

    public class GalleryFormDto
    {
        public int CoverImgId { get; set; }
        public List<IFormFile> Images { get; set; } = new();
    }
    public class GalleryUpdateDto
    {
        public int GalleryId { get; set; }
        public int? CoverImgId { get; set; }     // optional: category badalni ho to
        public IFormFile? Image { get; set; }    // optional: image replace karni ho to
    }
}