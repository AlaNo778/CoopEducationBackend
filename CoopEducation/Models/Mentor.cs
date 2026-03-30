using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class Mentor
{
    public int MentorId { get; set; }

    public int? CompanyId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Position { get; set; }

    public string? Department { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public virtual Company? Company { get; set; }

    public virtual ICollection<CoopPlacement> CoopPlacements { get; set; } = new List<CoopPlacement>();
}
