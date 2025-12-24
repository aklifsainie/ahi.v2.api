using ahis.template.identity.Contexts;
using ahis.template.identity.Interfaces;
using ahis.template.identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentResults;

namespace ahis.template.identity.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IdentityContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IdentityContext context,
            IConfiguration configuration,
            ILogger<AuthenticationService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(string userNameOrEmail, string password, bool rememberMe = false)
        {
            var user = await _userManager.FindByNameAsync(userNameOrEmail) ?? await _userManager.FindByEmailAsync(userNameOrEmail);
            if (user == null)
                return Result.Fail<AuthResponseDto>("Invalid credentials.");

            if (!user.IsActive || user.IsDeleted)
                return Result.Fail<AuthResponseDto>("User is not active.");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                    return Result.Fail<AuthResponseDto>("User is locked out.");

                if (signInResult.RequiresTwoFactor)
                    return Result.Ok(new AuthResponseDto { RequiresTwoFactor = true, UserId = user.Id.ToString() });

                return Result.Fail<AuthResponseDto>("Invalid credentials.");
            }

            // create tokens
            var accessToken = await GenerateJwtTokenAsync(user);
            var (refreshToken, refreshExpiresAt) = GenerateRefreshToken();

            // persist refresh token
            await StoreRefreshTokenAsync(user.Id, refreshToken, refreshExpiresAt);

            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                ExpiresInSeconds = int.Parse(_configuration["Jwt:AccessTokenExpirySeconds"] ?? "3600"),
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshExpiresAt,
                UserId = user.Id.ToString(),
                RequiresTwoFactor = false
            };

            return Result.Ok(response);
        }

        public async Task<Result> LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
                return Result.Fail("Logout failed.");
            }
        }

        public async Task<Result<AuthResponseDto>> RefreshTokenAsync(string userId, string refreshToken)
        {
            try
            {


                var stored = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == userId && x.Token == refreshToken && !x.IsRevoked);
                if (stored == null || stored.ExpiresAt < DateTime.UtcNow)
                    return Result.Fail<AuthResponseDto>("Invalid or expired refresh token.");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result.Fail<AuthResponseDto>("User not found.");

                // create new access token
                var accessToken = await GenerateJwtTokenAsync(user);
                var (newRefreshToken, newRefreshExpiresAt) = GenerateRefreshToken();

                // revoke old and store new
                stored.IsRevoked = true;
                _context.RefreshTokens.Update(stored);
                await StoreRefreshTokenAsync(user.Id, newRefreshToken, newRefreshExpiresAt);
                await _context.SaveChangesAsync();

                var response = new AuthResponseDto
                {
                    AccessToken = accessToken,
                    ExpiresInSeconds = int.Parse(_configuration["Jwt:AccessTokenExpirySeconds"] ?? "3600"),
                    RefreshToken = newRefreshToken,
                    RefreshTokenExpiresAt = newRefreshExpiresAt,
                    UserId = user.Id.ToString(),
                };

                return Result.Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RefreshTokenAsync failed");
                return Result.Fail<AuthResponseDto>("Failed to refresh token.");
            }
        }

        public async Task<Result<AuthResponseDto>> VerifyTwoFactorAsync(string userId, string provider, string code, bool rememberMachine = false)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Fail<AuthResponseDto>("User not found.");

            // TwoFactorAuthenticatorSignInAsync signature: (string code, bool rememberClient, bool rememberBrowser)
            var valid = await _signInManager.TwoFactorAuthenticatorSignInAsync(code, rememberMachine, rememberMachine);
            if (!valid.Succeeded)
            {
                return Result.Fail<AuthResponseDto>("Invalid two-factor verification code.");
            }

            // successful 2FA, return tokens
            var accessToken = await GenerateJwtTokenAsync(user);
            var (refreshToken, refreshExpiresAt) = GenerateRefreshToken();
            await StoreRefreshTokenAsync(user.Id, refreshToken, refreshExpiresAt);

            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                ExpiresInSeconds = int.Parse(_configuration["Jwt:AccessTokenExpirySeconds"] ?? "3600"),
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshExpiresAt,
                UserId = user.Id.ToString(),
                RequiresTwoFactor = false
            };

            return Result.Ok(response);
        }

        public async Task<Result> RevokeRefreshTokensAsync(string userId)
        {

            var tokens = await _context.RefreshTokens.Where(x => x.UserId == userId && !x.IsRevoked).ToListAsync();
            tokens.ForEach(t => t.IsRevoked = true);
            _context.RefreshTokens.UpdateRange(tokens);
            await _context.SaveChangesAsync();

            return Result.Ok();
        }

        #region Helpers

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"] ?? throw new InvalidOperationException("Jwt:SigningKey not configured")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "3600"));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "",
                audience: _configuration["Jwt:Audience"] ?? "",
                claims: claims,
                expires: expiry,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private (string token, DateTime expiresAt) GenerateRefreshToken()
        {
            var random = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);
            var token = Convert.ToBase64String(random);
            var expires = DateTime.UtcNow.AddDays(int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "30"));
            return (token, expires);
        }

        private async Task StoreRefreshTokenAsync(string userId, string token, DateTime expiresAt)
        {
            var rt = new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };
            _context.RefreshTokens.Add(rt);
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
