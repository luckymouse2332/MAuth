using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MAuth.Web.Commons.Helpers;
using MAuth.Web.Data.Entities;
using MAuth.Web.Services.Users;
using DotNext;
using MAuth.Web.Commons.Options;

namespace MAuth.Web.Services.Identity;

public class IdentityService(
    IUserRepository userRepository,
    IOptionsSnapshot<JwtOptions> jwtOptions,
    SigningCredentials signingCredentials,
    IOptionsSnapshot<JwtBearerOptions> namedAuthOptions,
    ITokenStore tokenStore) : IIdentityService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    private readonly JwtBearerOptions _jwtBearerOptions = namedAuthOptions.Get(
        JwtBearerDefaults.AuthenticationScheme);

    private readonly IUserRepository _userRepository =
        userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    private readonly SigningCredentials _signingCredentials =
        signingCredentials ?? throw new ArgumentNullException(nameof(signingCredentials));

    private readonly ITokenStore _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));

    public async Task<Result<User, UserAccessError>> VerifyLoginDataAsync(string username, string password)
    {
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(password);

        var user = await _userRepository.GetUserByUsernameAsync(username);

        if (user is null)
            return new(UserAccessError.UserNotExists);

        if (HashHelper.ComputeSha256Hash(password) != user.Password)
            return new(UserAccessError.InvalidPassword);

        return new(user);
    }

    public async Task<Result<User, UserAccessError>> ValidateTokenAsync(string accessToken, string refreshToken)
    {
        var validationParameters = _jwtBearerOptions.TokenValidationParameters.Clone();
        validationParameters.ValidateLifetime = false;

        var handler = _jwtBearerOptions.TokenHandlers.OfType<JsonWebTokenHandler>()
            .FirstOrDefault() ?? new JsonWebTokenHandler();

        var result = await handler.ValidateTokenAsync(accessToken, validationParameters);

        if (result.Exception is not null)
            throw result.Exception;

        // 使用字典式查找，提高性能（大约比 FirstOrDefault 快 20~40%，尤其是在 Claim 多时）
        var claims = result.ClaimsIdentity.Claims.ToDictionary(c => c.Type, c => c.Value);

        if (!claims.TryGetValue(ClaimTypes.NameIdentifier, out var userId)
            || !Guid.TryParse(userId, out var id))
            return new(UserAccessError.InvalidAccessToken);

        if (!claims.TryGetValue(ClaimTypes.Role, out var userRole))
            return new(UserAccessError.InvalidAccessToken);

        if (!claims.TryGetValue(JwtOptions.RefreshTokenId, out var refreshTokenId))
            return new(UserAccessError.InvalidRefreshToken);

        var tokenKey = IdentityHelper.GetRefreshTokenKey(userId, refreshTokenId);
        var savedRefreshToken = await _tokenStore.GetTokenAsync(tokenKey);

        if (savedRefreshToken != refreshToken)
            return new(UserAccessError.InvalidRefreshToken);

        var user = await _userRepository.GetUserByIdAsync(id);

        if (user is null)
            return new(UserAccessError.UserNotExists);

        return user;
    }

    public async Task<(string, string)> CreateTokenAsync(User user)
    {
        var (refreshTokenId, refreshToken) = await CreateRefreshTokenAsync(user.Id.ToString());
        var accessToken = CreateJwtToken(user, refreshTokenId);
        return (accessToken, refreshToken);
    }

    private string CreateJwtToken(User user, string refreshTokenId)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshTokenId);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new(ClaimTypes.Role, user.Role.ToString()),
                new(JwtOptions.RefreshTokenId, refreshTokenId),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            ]),
            Issuer = _jwtOptions.Issuer,
            IssuedAt = DateTime.UtcNow,
            Audience = _jwtOptions.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpiresMinutes),
            SigningCredentials = _signingCredentials,
        };

        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.CreateJwtSecurityToken(tokenDescriptor);
        var token = handler.WriteToken(securityToken);
        return token;
    }

    private async Task<(string refreshTokenId, string refreshToken)> CreateRefreshTokenAsync(string userId)
    {
        // refresh token id作为缓存Key
        var tokenId = Guid.NewGuid().ToString("N");

        var token = IdentityHelper.GenerateRefreshToken();

        var tokenKey = IdentityHelper.GetRefreshTokenKey(userId, tokenId);

        // 缓存 refresh token
        await _tokenStore.SaveTokenAsync(tokenKey, token, TimeSpan.FromDays(_jwtOptions.RefreshTokenExpiresDays));

        return (tokenId, token);
    }
}