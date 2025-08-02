namespace AqbaApp.Model.Authorization
{
    public class RefreshModel(string refreshToken)
    {
        public string RefreshToken { get; set; } = refreshToken;
    }
}
