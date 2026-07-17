using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class StudentDocument
{
    public int DocId { get; set; }

    public int? StudentId { get; set; }

    public int? DocTypeId { get; set; }

    public int? PlacementId { get; set; }

    public string? FileName { get; set; }

    public string? FileSize { get; set; }

    public DateTime? UploadedAt { get; set; }

    public string? RealFileName { get; set; }

    public virtual DocumentType? DocType { get; set; }

    public virtual CoopPlacement? Placement { get; set; }

    public virtual Student? Student { get; set; }
}
