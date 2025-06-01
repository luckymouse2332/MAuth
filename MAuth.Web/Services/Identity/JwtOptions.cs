namespace MAuth.Web.Services.Identity;

public class JwtOptions
{
    public readonly static string RefreshTokenIdClaimType = "refresh_token_id";
    public readonly static string UserIdClaimType = "user_id";

    public const string Name = "Jwt";

    public readonly static double DefaultAccessTokenExpiresMinutes = 30d;
    public readonly static double DefaultRefreshTokenExpiresDays = 7d;

    public string Audience { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public double AccessTokenExpiresMinutes { get; set; } = DefaultAccessTokenExpiresMinutes;

    public double RefreshTokenExpiresDays { get; set; } = DefaultRefreshTokenExpiresDays;

    public string JweEncryptKey { get; set; } = string.Empty;
}
