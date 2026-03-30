using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class CoopPlacement
{
    public int PlacementId { get; set; }

    public int StudentId { get; set; }

    public int CompanyId { get; set; }

    public int? MentorId { get; set; }

    public string? JobTitle { get; set; }

    public string? JobDescription { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? AcademicYear { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual Mentor? Mentor { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual ICollection<StudentDocument> StudentDocuments { get; set; } = new List<StudentDocument>();
}
