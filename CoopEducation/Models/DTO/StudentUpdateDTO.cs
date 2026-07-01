namespace CoopEducation.Dtos;

public class UpdateStudentInfoDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int MajorId { get; set; }
    public decimal? Gpax { get; set; }
    public short? TotalCredits { get; set; }
}

public class UpdateStudentContactDto
{
    public string? Email { get; set; }
    public string? PhoneHome { get; set; }
    public string? PhoneMobile { get; set; }
    public string? Facebook { get; set; }
    public string? LineId { get; set; }
}

public class UpdateStudentAddressDto
{
    public string? HouseNo { get; set; }
    public string? VillageNo { get; set; }
    public string? Alley { get; set; }
    public string? Road { get; set; }
    public string? SubDistrict { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
    public string? Postcode { get; set; }
}

public class UpdateStudentDto
{
    public UpdateStudentInfoDto Info { get; set; }
    public UpdateStudentContactDto Contact { get; set; }
    public UpdateStudentAddressDto Address { get; set; }
}