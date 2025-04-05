namespace MAuth.Web.Configurations
{
    public class JwtOptions
    {
        public const string Name = "Jwt";
        public readonly static double DefaultExpiresMinutes = 30d;

        public string Audience { get; set; } = string.Empty;

        public string Issuer { get; set; } = string.Empty;

        public double ExpiresMinutes { get; set; } = DefaultExpiresMinutes;

        public string JweEncryptKey { get; set; } = string.Empty;
    }
}
