using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class SupervisionAppointmentDate
{
    public int? ScheduleId { get; set; }

    public int? StudentId { get; set; }

    public int? TeacherId { get; set; }

    public string? SupervisionModel { get; set; }

    public DateOnly? AppointmentDate { get; set; }

    public TimeOnly? AppointmentTime { get; set; }

    public string? AppointmentLocation { get; set; }

    public int AppointmentConfirmation { get; set; }
}
