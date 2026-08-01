namespace CoopEducation.Models.DTO
{
    public class GetAppointmentSlotDTO
    {
        public int SlotId { get; set; }
        public DateOnly AvailableDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string SupervisionModel { get; set; } = null!;
        public string? Location { get; set; }
        public int? MaxStudents { get; set; }
        public int? BookedStudents { get; set; }
        public string? SlotStatus { get; set; }
        public string? Remark { get; set; }
    }
}
