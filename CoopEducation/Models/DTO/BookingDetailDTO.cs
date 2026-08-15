namespace CoopEducation.Models.DTO
{
    public class BookingDetailDTO
    {
        public int AppointmentId { get; set; }
        public int SlotId { get; set; }
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public string? AppointmentStatus { get; set; }
        public string? StudentNote { get; set; }
        public string? TeacherNote { get; set; }
        public DateTime? BookedAt { get; set; }
        public virtual TeacherAvailableSlot Slot { get; set; } = null!;

    }
}
