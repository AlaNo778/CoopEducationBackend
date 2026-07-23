using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class TeacherDocument
{
    public int DocId { get; set; }

    public int TeacherId { get; set; }

    public int DocTypeId { get; set; }

    public int? StudentId { get; set; }

    public string? FileName { get; set; }

    public DateTime? UploadedAt { get; set; }
}
