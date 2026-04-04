namespace CoopEducation.Models.DTO
{
    public class SetLogDocDTO
    {
        public int? StudentId { get; set; }
        public int? DocTypeId { get; set; }
        public string? FileName { get; set; }
        public int? PlacementId { get; set; }
        public DateTime? UploadedAt { get; set; }

    }
}
