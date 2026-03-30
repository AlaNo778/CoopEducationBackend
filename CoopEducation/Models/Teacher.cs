using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class Teacher
{
    public int TeacherId { get; set; }

    public int? UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Position { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Advisorship> Advisorships { get; set; } = new List<Advisorship>();

    public virtual User? User { get; set; }
}
