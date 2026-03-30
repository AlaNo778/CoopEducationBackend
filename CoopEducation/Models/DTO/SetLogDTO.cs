namespace CoopEducation.Models.DTO
{
    public class SetLogDTO
    {

        public string? Method { get; set; }

        public string? ApiEndpoint { get; set; }

        public string? Request { get; set; }

        public string? Response { get; set; }

        public string? StatusCode { get; set; }

        public int? CreateBy { get; set; }
    }
}
