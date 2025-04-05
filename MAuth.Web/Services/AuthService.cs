using AutoMapper;
using MAuth.Web.Configurations;
using MAuth.Web.Data.Repositories;
using MAuth.Web.Models.DTOs;
using MAuth.Web.Models.Entities;
using MAuth.Web.Models.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MAuth.Web.Services
{
    public class AuthService(
        IUserRepository userRepository,
        IOptionsSnapshot<JwtOptions> jwtOptions, 
        SigningCredentials signingCredentials,
        IMapper mapper) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;

        private readonly IMapper _mapper = mapper;

        private readonly JwtOptions _jwtOptions = jwtOptions.Value;

        private readonly SigningCredentials _signingCredentials = signingCredentials;

        public async Task<string> LoginAsync(string username, string password)
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

            return CreateJwtToken(userDto);
        }

        public Task<AuthTokenDto> CreateAuthTokenAsync(UserDto user)
        {
            throw new NotImplementedException();
        }

        public Task<AuthTokenDto> RefreshAuthTokenAsync(AuthTokenDto token)
        {
            throw new NotImplementedException();
        }

        private string CreateJwtToken(UserDto userDto)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new (ClaimTypes.Name, userDto.Username), 
                    new (ClaimTypes.Role, userDto.Role)]),
                Issuer = _jwtOptions.Issuer,
                IssuedAt = DateTime.UtcNow,
                Audience = _jwtOptions.Audience,
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresMinutes),
                SigningCredentials = _signingCredentials,
            };

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateJwtSecurityToken(tokenDescriptor);
            var token = handler.WriteToken(securityToken);
            return token;
        }
    }
}
