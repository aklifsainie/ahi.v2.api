using FluentResults;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ahis.template.identity.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Result<AuthResponseDto>> LoginAsync(string userNameOrEmail, string password, bool rememberMe = false);
        Task<Result> LogoutAsync();
        Task<Result<AuthResponseDto>> RefreshTokenAsync(string userId, string refreshToken);
        Task<Result<AuthResponseDto>> VerifyTwoFactorAsync(string userId, string provider, string code, bool rememberMachine = false);
        Task<Result> RevokeRefreshTokensAsync(string userId);

        JwtPayload DecodeToken(string token);
        IEnumerable<Claim> GetClaims(string token);
    }

    public record AuthResponseDto
    {
        public string AccessToken { get; init; } = string.Empty;
        public int ExpiresInSeconds { get; init; }
        public string RefreshToken { get; init; } = string.Empty;
        public DateTime RefreshTokenExpiresAt { get; init; }
        public string UserId { get; init; } = string.Empty;
        public bool RequiresTwoFactor { get; init; }
    }
}
