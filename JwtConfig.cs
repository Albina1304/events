namespace events
{
    public class JwtConfig
    {
        public string Secret { get; set; }
        public int ExpirationInMinutes { get; set; }
        public string CookieName { get; set; }
    }
}
