namespace CoopEducation.Models.DTO
{
    public class UpdateAndAssignMentorDTO
    {
        public int MentorId { get; set; }
        public int? CompanyId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Position { get; set; }
        public string? Department { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
