using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Fax { get; set; }

    public string? Email { get; set; }

    public string? HrName { get; set; }

    public string? Address { get; set; }

    public DateTime? CreateAd { get; set; }

    public virtual ICollection<CoopPlacement> CoopPlacements { get; set; } = new List<CoopPlacement>();

    public virtual ICollection<Mentor> Mentors { get; set; } = new List<Mentor>();
}
