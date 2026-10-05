namespace st_xaviers_website_api.Models
{
    public class StudentRegistrationModel: BaseModel
    {
        public int? StudentRegistrationId { get; set; }
        public string? StudentName { get; set; }
        public string? ClassApplyingFor { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? FathersName { get; set; }
        public string? MothersName { get; set; }
        public string? LastSchoolAttended { get; set; }
        public decimal? Percentage { get; set; }
        public string? ContactNumber { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? Email { get; set; }
        public string? Nationality { get; set; }
        public string? PermanentAddress { get; set; }
    }
}
