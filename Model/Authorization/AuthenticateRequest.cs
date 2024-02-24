namespace AqbaApp.Model.Authorization
{
    public class AuthenticateRequest(string email, string password)
    {
        public string Email { get; set; } = email;
        public string Password { get; set; } = password;
    }
}