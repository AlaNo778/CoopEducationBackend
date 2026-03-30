using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class ApiLog
{
    public int LogId { get; set; }

    public string? Method { get; set; }

    public string? ApiEndpoint { get; set; }

    public string? Request { get; set; }

    public string? Response { get; set; }

    public string? StatusCode { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? CreateBy { get; set; }
}
