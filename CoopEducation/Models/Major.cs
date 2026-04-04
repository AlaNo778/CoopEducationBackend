using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class Major
{
    public int MajorId { get; set; }

    public string MajorName { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
