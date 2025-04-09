using AutoMapper;
using MAuth.Web.Configurations;
using MAuth.Web.Data.Repositories;
using MAuth.Web.Models.DTOs;
using MAuth.Web.Models.Entities;
using MAuth.Web.Models.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace MAuth.Web.Services
{
    public class AuthService(
        IUserRepository userRepository,
        IOptionsSnapshot<JwtOptions> jwtOptions, 
        SigningCredentials signingCredentials,
        IDistributedCache distributedCache,
        IOptionsSnapshot<JwtBearerOptions> jwtBearerOptions,
        IMapper mapper) : IAuthService
    {
        private const string RefreshTokenIdClaimType = "refresh_token_id";

        private readonly IUserRepository _userRepository = userRepository;

        private readonly IMapper _mapper = mapper;

        private readonly JwtOptions _jwtOptions = jwtOptions.Value;

        private readonly SigningCredentials _signingCredentials = signingCredentials;

        private readonly IDistributedCache _distributedCache = distributedCache;

        private readonly JwtBearerOptions _jwtBearerOptions = jwtBearerOptions.Value;

        public async Task<AuthTokenDto> LoginAsync(string username, string password)
        {
            var userResult = await _userRepository.GetByUsernameAsync(username);

            if (userResult == null 
                || !BCrypt.Net.BCrypt.EnhancedVerify(password, userResult.Password))
            {
                throw new CustomException(
                    StatusCodes.Status401Unauthorized, "用户名或密码错误!");
            }

            if (userResult.Status == UserStatus.Banned)
            {
                throw new CustomException(StatusCodes.Status401Unauthorized, "账户已封禁!");
            }

            // 数据脱敏,防止密码被带进去
            var userDto = _mapper.Map<UserDto>(userResult);

            return await CreateAuthTokenAsync(userDto);
        }

        public async Task<UserDto> GetUserInfoAsync(ClaimsPrincipal user)
        {
            var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;
            
            CustomException.ThrowIf(!Guid.TryParse(userId, out var id),
                StatusCodes.Status400BadRequest,
                "Invalid userId");

            var userEntity = await _userRepository.GetByIdAsync(id);
            return _mapper.Map<UserDto>(userEntity);
        }

        public async Task<AuthTokenDto> RefreshAuthTokenAsync(AuthTokenDto token)
        {
            // 复制，防止影响其他的逻辑
            var validationParameters = _jwtBearerOptions.TokenValidationParameters.Clone();
            // 不校验生命周期
            validationParameters.ValidateLifetime = false;
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.JweEncryptKey));

            var handler = _jwtBearerOptions.TokenHandlers.OfType<JsonWebTokenHandler>()
                .FirstOrDefault() ?? new JsonWebTokenHandler();
            try
            {
                // 先验证一下，jwt是否真的有效
                var result = await handler.ValidateTokenAsync(token.AccessToken, validationParameters);
                var identity = result.ClaimsIdentity;

                // 从认证结果中拿出属性
                var userId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var refreshTokenId = identity.Claims.FirstOrDefault(c => c.Type == RefreshTokenIdClaimType)?.Value;
                var userRole = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                CustomException.ThrowIf(!Guid.TryParse(userId, out var id), 
                    StatusCodes.Status400BadRequest,
                    "Invalid userId");
                
                CustomException.ThrowIfNull(refreshTokenId, StatusCodes.Status400BadRequest);
                CustomException.ThrowIfNull(userRole, StatusCodes.Status400BadRequest);

                var refreshTokenKey = GetRefreshTokenKey(id.ToString(), refreshTokenId);
                var refreshToken = await _distributedCache.GetStringAsync(refreshTokenKey);

                CustomException.ThrowIf(refreshToken != token.RefreshToken,
                    StatusCodes.Status400BadRequest,
                    "Invalid refresh token");

                // refresh token用过了记得清除掉
                await _distributedCache.RemoveAsync(refreshTokenKey);

                // 这里应该从缓存拿数据
                var user = await _userRepository.GetByIdAsync(id);
                var userDto = _mapper.Map<UserDto>(user);

                return await CreateAuthTokenAsync(userDto);
            }
            catch(CustomException ex)
            {
                Log.Warning(ex.Message);
                throw new CustomException(StatusCodes.Status401Unauthorized, "Invalid token pattern!");
            }
            catch (Exception ex)
            {
                Log.Warning(ex.ToString());
                throw new CustomException(StatusCodes.Status400BadRequest,
                    "Invalid access token");
            }
        }

        public async Task<AuthTokenDto> CreateAuthTokenAsync(UserDto user)
        {
            // 先创建refresh token
            var (refreshTokenId, refreshToken) = await CreateRefreshTokenAsync(user.Id);

            var result = new AuthTokenDto
            {
                RefreshToken = refreshToken,
                // 再签发Jwt
                AccessToken = CreateJwtToken(user, refreshTokenId)
            };

            return result;
        }

        private string CreateJwtToken(UserDto userDto, string refreshTokenKey)
        {
            ArgumentNullException.ThrowIfNull(userDto);
            ArgumentException.ThrowIfNullOrWhiteSpace(refreshTokenKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new (ClaimTypes.Role, userDto.Role),
                    new (RefreshTokenIdClaimType, refreshTokenKey),
                    new (ClaimTypes.NameIdentifier, userDto.Id)
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

            // 生成refresh token
            var rnBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(rnBytes);
            var token = Convert.ToBase64String(rnBytes);

            // 设置refresh token的过期时间
            var options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromDays(_jwtOptions.RefreshTokenExpiresDays));

            // 缓存 refresh token
            await _distributedCache.SetStringAsync(GetRefreshTokenKey(userId, tokenId), token, options);

            return (tokenId, token);
        }

        private static string GetRefreshTokenKey(string userId, string refreshTokenId)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrWhiteSpace(refreshTokenId)) throw new ArgumentNullException(nameof(refreshTokenId));

            return $"{userId}:{refreshTokenId}";
        }
    }
}
