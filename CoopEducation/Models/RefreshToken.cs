using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime Expiry { get; set; }

    public bool? Revoked { get; set; }

    public DateTime? CreatedAt { get; set; }
}
