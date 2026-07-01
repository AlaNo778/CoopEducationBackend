namespace CoopEducation.Models.DTO
{
    public class CoopAndMentorInfoDTO
    {
        public int StudentId { get; set; }
        public CoopPlacementDTO? Coop { get; set; }
    }
    public class CoopPlacementDTO
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? CompanyPhone { get; set; }
        public string? CompanyFax { get; set; }
        public string? CompanyEmail { get; set; }
        public string? HrName { get; set; }
        public string? Address { get; set; }

        public string? JobTitle { get; set; }
        public string? JobDescription { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? AcademicYear { get; set; }

        public MentorDTO? Mentor { get; set; }
    }
    public class MentorDTO
    {
        public int MentorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Position { get; set; }
        public string? Department { get; set; }
        public string? MentorPhone { get; set; }
        public string? MentorEmail { get; set; }
    }
}
