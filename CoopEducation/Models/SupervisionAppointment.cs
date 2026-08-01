using System;
using System.Collections.Generic;

namespace CoopEducation.Models;

public partial class SupervisionAppointment
{
    public int AppointmentId { get; set; }

    public int SlotId { get; set; }

    public int StudentId { get; set; }

    public int TeacherId { get; set; }

    public string? AppointmentStatus { get; set; }

    public string? StudentNote { get; set; }

    public string? TeacherNote { get; set; }

    public DateTime? BookedAt { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public virtual TeacherAvailableSlot Slot { get; set; } = null!;
}
