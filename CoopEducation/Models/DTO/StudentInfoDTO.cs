namespace CoopEducation.Models.DTO
{
    public class StudentInfoDTO
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Faculty { get; set; }
        public decimal? Gpax { get; set; }
        public short? TotalCredits { get; set; }
        public string? MajorName { get; set; }
        public string? HouseNo { get; set; }
        public string? Road { get; set; }
        public string? Alley { get; set; }
        public string? VillageNo { get; set; }
        public string? SubDistrict { get; set; }
        public string? District { get; set; }
        public string? Province { get; set; }
        public string? Postcode { get; set; }
        public string? PhoneHome { get; set; }
        public string? PhoneMobile { get; set; }
        public string? Facebook { get; set; }
        public string? LineId { get; set; }
        public string? Advisor { get; set; }
    }
}
