using CoopEducation.Dtos;
using CoopEducation.Models;
using Microsoft.EntityFrameworkCore;

namespace CoopEducation.Services;


public interface IStudentUpdateService
{
    Task<(bool Success, string Message)> UpdateStudentAsync(int userId, UpdateStudentDto dto);
}

public class StudentUpdateService : IStudentUpdateService
{
    private readonly CoopEducationDbContext _context;

    public StudentUpdateService(CoopEducationDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message)> UpdateStudentAsync(int userId, UpdateStudentDto dto)
    {
        var student = await _context.Students
            .Include(s => s.StudentContact)
            .Include(s => s.StudentAddress)
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (student == null)
            return (false, "Student not found.");

        if (dto.Info != null)
        {
            var majorExists = await _context.Majors.AnyAsync(m => m.MajorId == dto.Info.MajorId);
            if (!majorExists)
                return (false, "Major not found.");

            student.FirstName = dto.Info.FirstName;
            student.LastName = dto.Info.LastName;
            student.MajorId = dto.Info.MajorId;
            student.Gpax = dto.Info.Gpax;
            student.TotalCredits = dto.Info.TotalCredits;
        }

        if (dto.Contact != null)
        {
            student.StudentContact ??= new StudentContact { StudentId = student.StudentId };
            if (student.StudentContact.ContactId == 0)
                _context.StudentContacts.Add(student.StudentContact);

            student.Email = dto.Contact.Email;
            student.StudentContact.PhoneHome = dto.Contact.PhoneHome;
            student.StudentContact.PhoneMobile = dto.Contact.PhoneMobile;
            student.StudentContact.Facebook = dto.Contact.Facebook;
            student.StudentContact.LineId = dto.Contact.LineId;
        }

        if (dto.Address != null)
        {
            student.StudentAddress ??= new StudentAddress { StudentId = student.StudentId };
            if (student.StudentAddress.AddressId == 0)
                _context.StudentAddresses.Add(student.StudentAddress);

            student.StudentAddress.HouseNo = dto.Address.HouseNo;
            student.StudentAddress.VillageNo = dto.Address.VillageNo;
            student.StudentAddress.Alley = dto.Address.Alley;
            student.StudentAddress.Road = dto.Address.Road;
            student.StudentAddress.SubDistrict = dto.Address.SubDistrict;
            student.StudentAddress.District = dto.Address.District;
            student.StudentAddress.Province = dto.Address.Province;
            student.StudentAddress.Postcode = dto.Address.Postcode;
        }

        await _context.SaveChangesAsync();
        return (true, "Student information updated successfully.");
    }
}