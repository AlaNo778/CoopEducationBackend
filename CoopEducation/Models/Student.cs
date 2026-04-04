using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public int? UserId { get; set; }

    public string StudentCode { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Faculty { get; set; }

    public decimal? Gpax { get; set; }

    public short? TotalCredits { get; set; }

    public int? MajorId { get; set; }

    public virtual ICollection<Advisorship> Advisorships { get; set; } = new List<Advisorship>();

    public virtual ICollection<CoopPlacement> CoopPlacements { get; set; } = new List<CoopPlacement>();

    public virtual Major? Major { get; set; }

    public virtual StudentAddress? StudentAddress { get; set; }

    public virtual StudentContact? StudentContact { get; set; }

    public virtual ICollection<StudentDocument> StudentDocuments { get; set; } = new List<StudentDocument>();

    public virtual User? User { get; set; }
}
