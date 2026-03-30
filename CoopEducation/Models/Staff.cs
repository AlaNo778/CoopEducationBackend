using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public int? UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Position { get; set; }

    public virtual User? User { get; set; }
}
