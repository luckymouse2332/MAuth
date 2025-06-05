namespace MAuth.Web.Commons.Options;

public class JwtOptions
{
    public const string RefreshTokenId = "refresh_token_id";

    public const string Name = "Jwt";

    public static readonly double DefaultAccessTokenExpiresMinutes = 30d;
    public static readonly double DefaultRefreshTokenExpiresDays = 7d;

    public string Audience { get; init; } = string.Empty;

    public string Issuer { get; init; } = string.Empty;

    public double AccessTokenExpiresMinutes { get; init; } = DefaultAccessTokenExpiresMinutes;

    public double RefreshTokenExpiresDays { get; init; } = DefaultRefreshTokenExpiresDays;

    public string JweEncryptKey { get; init; } = string.Empty;
}
