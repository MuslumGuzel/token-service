namespace token_service.Models
{
    public class AuthToken
    {
        public string AccessToken { get; set; } = null!;
        public double Expires { get; set; }
    }
}