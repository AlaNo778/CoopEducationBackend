using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class Advisorship
{
    public int AdvisorshipId { get; set; }

    public int? StudentId { get; set; }

    public int? TeacherId { get; set; }

    public string AcademicYear { get; set; } = null!;

    public DateTime? AssignedAt { get; set; }

    public virtual Student? Student { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
