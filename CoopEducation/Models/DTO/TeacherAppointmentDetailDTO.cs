namespace CoopEducation.Models.DTO
{
    public class TeacherAppointmentDetailDTO
    {
        public int AppointmentId { get; set; }
        public int SlotId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string? AppointmentStatus { get; set; }
        public string? StudentNote { get; set; }
        public string? TeacherNote { get; set; }
        public DateTime? BookedAt { get; set; }
        public virtual TeacherAvailableSlot Slot { get; set; } = null!;
    }
}
