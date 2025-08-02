namespace AqbaApp.Model.Authorization
{
    public class AuthenticateResponse()
    {
        public string? AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
    }
}
