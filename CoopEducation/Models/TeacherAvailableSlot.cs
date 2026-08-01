using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class TeacherAvailableSlot
{
    public int SlotId { get; set; }

    public int TeacherId { get; set; }

    public DateOnly AvailableDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string SupervisionModel { get; set; } = null!;

    public string? Location { get; set; }

    public int? MaxStudents { get; set; }

    public int? BookedStudents { get; set; }

    public string? SlotStatus { get; set; }

    public string? Remark { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<SupervisionAppointment> SupervisionAppointments { get; set; } = new List<SupervisionAppointment>();
}
