namespace CoopEducation.Models.Response
{
    public class ResponseMessage<T>
    {
        public bool isError { get; set; }
        public string? message { get; set; }
        public string? code { get; set; }
        public T? data { get; set; }
    }
}
