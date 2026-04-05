namespace CoopEducation.Models.DTO
{
    public class AdviseeStudentsDTO
    {
        public string StudentCode { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Faculty { get; set; }
        public decimal? Gpax { get; set; }
        public short? TotalCredits { get; set; } 
        public string? MajorName { get; set; }
        public string? PhoneHome { get; set; }
        public string? PhoneMobile { get; set; }
        public string? Facebook { get; set; }
        public string? LineId { get; set; }
        public string MentorFirstName { get; set; } = null!;
        public string MentorLastName { get; set; } = null!;
        public string? Position { get; set; }
        public string? Department { get; set; }
        public string? Phone { get; set; }
        public string? MentorEmail { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? JobTitle { get; set; }
        public string? JobDescription { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? AcademicYear { get; set; }
    }
}
