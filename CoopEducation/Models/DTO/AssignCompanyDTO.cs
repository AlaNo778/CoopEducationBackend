namespace CoopEducation.Models.DTO
{
    public class AssignCompanyDTO
    {
        public int CompanyId { get; set; }
        public int? MentorId { get; set; }
        public string JobTitle { get; set; } = null!;
        public string JobDescription { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? AcademicYear { get; set; }
    }
}