using ahis.template.domain.Models.ViewModels.AuthenticationVM;
using FluentResults;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ahis.template.identity.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Result<AuthenticationResponseVM>> CheckAccountStateByEmailAsync(string email);
        Task<Result<AuthenticationResponseVM>> LoginAsync(string userNameOrEmail, string password, bool rememberMe = false);
        //Task<Result> LogoutAsync(string refreshToken);
        Task LogoutAsync(string refreshToken);
        Task<Result<AuthenticationResponseVM>> RefreshTokenAsync(string userId, string refreshToken);
        Task<Result<AuthenticationResponseVM>> VerifyTwoFactorAsync(string userId, string code, bool rememberMachine = false);
        Task<Result> RevokeRefreshTokensAsync(string userId);

        JwtPayload DecodeToken(string token);
        IEnumerable<Claim> GetClaims(string token);
    }

}
