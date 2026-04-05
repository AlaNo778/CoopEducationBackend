using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class StudentAddress
{
    public int AddressId { get; set; }

    public int? StudentId { get; set; }

    public string? HouseNo { get; set; }

    public string? SubDistrict { get; set; }

    public string? District { get; set; }

    public string? Province { get; set; }

    public string? Postcode { get; set; }

    public string? VillageNo { get; set; }

    public string? Alley { get; set; }

    public string? Road { get; set; }

    public virtual Student? Student { get; set; }
}
