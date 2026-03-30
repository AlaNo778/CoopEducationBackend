using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class DocumentType
{
    public int DocTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string DocName { get; set; } = null!;

    public bool? IsRequired { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<StudentDocument> StudentDocuments { get; set; } = new List<StudentDocument>();
}
