using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace st_xaviers_website_api.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        public string? UploadPhoto(IFormFile photo, string folder)
        {
            if (photo == null || photo.Length == 0) return null;

            using var stream = photo.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(photo.FileName, stream),
                Folder = folder, // jaise "topper-photos"
                PublicId = new Random().Next(100000, 999999).ToString()
            };

            var result = _cloudinary.Upload(uploadParams);
            return result.SecureUrl?.ToString();
        }
    }
}