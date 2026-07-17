namespace CoopEducation.Models.DTO
{
    public class CompanyDTO
    {
        public int? CompanyId { get; set; }

        public string CompanyName { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Fax { get; set; }

        public string? Email { get; set; }

        public string? HrName { get; set; }

        public string? Address { get; set; }
    }
}
