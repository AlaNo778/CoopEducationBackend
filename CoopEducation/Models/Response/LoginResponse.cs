namespace CoopEducation.Models.Response
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string CsrfToken { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
