using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class StudentContact
{
    public int ContactId { get; set; }

    public int? StudentId { get; set; }

    public string? PhoneHome { get; set; }

    public string? PhoneMobile { get; set; }

    public string? Facebook { get; set; }

    public string? LineId { get; set; }

    public virtual Student? Student { get; set; }
}
