namespace st_xaviers_website_api.Models
{
    public class EnquiryFormModel: BaseModel
    {
        public int? EnquiryFormId { get; set; }
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string? FathersName { get; set; } = string.Empty;
        public string? MobileNumber { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? ClassApplyingFor { get; set; }
        public string? CurrentSchoolName { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        
    }
}
