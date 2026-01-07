using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using FluentResults;
using ahis.template.identity.Interfaces;
using ahis.template.identity.Models.Entities;
using ahis.template.identity.Models.DTOs;


namespace ahis.template.identity.Services
{
    

    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IConfiguration configuration,
            ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _configuration = configuration;
            _logger = logger;
        }

        // 1. Register new user (no password yet)
        public async Task<Result<string>> RegisterAsync(string email, string userName, string callbackBaseUrl)
        {
            try
            {
                var existing = await _userManager.FindByEmailAsync(email);
                if (existing != null)
                {
                    _logger.LogWarning("Registration failed: email already exists {Email}", email);
                    return Result.Fail<string>("Email is already registered.");
                }

                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = false,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    _logger.LogWarning("User creation failed: {Errors}", string.Join(';', createResult.Errors.Select(e => e.Description)));
                    var errors = string.Join(';', createResult.Errors.Select(e => e.Description));
                    return Result.Fail<string>(errors);
                }

                // Send confirmation email
                var emailSendResult = await SendEmailConfirmationAsync(user, callbackBaseUrl);
                if (!emailSendResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to send confirmation email to {Email}", email);
                    return Result.Fail<string>("User created but failed to send confirmation email.");
                }

                return Result.Ok(user.Id.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during RegisterAsync");
                return Result.Fail<string>("An unexpected error occurred while registering user.");
            }
        }

        // 2. Send email for verification
        public async Task<Result> SendEmailConfirmationAsync(ApplicationUser user, string callbackBaseUrl)
        {
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                // build callback url: e.g. {callbackBaseUrl}/api/account/confirm-email?userId={userId}&token={token}
                var callbackUrl = BuildCallbackUrl(callbackBaseUrl, "account/confirm-email", new Dictionary<string, string>
                {
                    ["userId"] = user.Id.ToString(),
                    ["token"] = encodedToken
                });

                var subject = "Confirm your email";
                var message = $"Please confirm your account by <a href=\"{callbackUrl}\">clicking here</a>.";

                await _emailSender.SendEmailAsync(user.Email!, subject, message);

                _logger.LogInformation("Email confirmation sent to {Email}", user.Email);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendEmailConfirmationAsync failed");
                return Result.Fail("Failed to send confirmation email.");
            }
        }

        // 3. Confirm email
        public async Task<Result> ConfirmEmailAsync(string userId, string encodedToken)
        {
            try
            {

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result.Fail("User not found.");

                var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedToken));
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    var errors = string.Join(';', result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Email confirmation failed for user {UserId}: {Errors}", userId, errors);
                    return Result.Fail(errors);
                }

                user.EmailVerifiedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ConfirmEmailAsync error");
                return Result.Fail("An unexpected error occurred while confirming email.");
            }
        }

        // 4. Set password on first login (user has no password yet)
        public async Task<Result> SetPasswordFirstTimeAsync(string userId, string password)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result.Fail("User not found.");

                // If user already has password, prevent using this method
                var hasPassword = await _userManager.HasPasswordAsync(user);
                if (hasPassword)
                    return Result.Fail("Password already set. Use change password flow.");

                var addPasswordResult = await _userManager.AddPasswordAsync(user, password);
                if (!addPasswordResult.Succeeded)
                {
                    var errors = string.Join(';', addPasswordResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("AddPassword failed for user {UserId}: {Errors}", userId, errors);
                    return Result.Fail(errors);
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetPasswordFirstTimeAsync error");
                return Result.Fail("An unexpected error occurred while setting password.");
            }
        }

        // Generate password reset token (encoded)
        public async Task<Result<string>> GeneratePasswordResetTokenAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return Result.Fail<string>("User not found.");

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                return Result.Ok(encoded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GeneratePasswordResetTokenAsync failed");
                return Result.Fail<string>("Failed to generate password reset token.");
            }
        }

        public async Task<Result<string>> SendEmailForgotPasswordAsync(string email, string callbackBaseUrl)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return Result.Fail<string>("User not found.");

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                // build callback url: e.g. {callbackBaseUrl}/api/account/confirm-email?userId={userId}&token={token}
                var callbackUrl = BuildCallbackUrl(callbackBaseUrl, "account/reset-password", new Dictionary<string, string>
                {
                    ["userId"] = user.Id.ToString(),
                    ["token"] = encoded
                });

                var subject = "Reset password";
                var message = $"Please reset your password by <a href=\"{callbackUrl}\">clicking here</a>.";

                await _emailSender.SendEmailAsync(user.Email!, subject, message);

                return Result.Ok(encoded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GeneratePasswordResetTokenAsync failed");
                return Result.Fail<string>("Failed to generate password reset token.");
            }
        }

        public async Task<Result> ResetPasswordAsync(string userId, string encodedToken, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result.Fail("User not found.");

                var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedToken));
                var resetResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (!resetResult.Succeeded)
                {
                    var errors = string.Join(';', resetResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("ResetPasswordAsync failed for user {UserId}: {Errors}", userId, errors);
                    return Result.Fail(errors);
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ResetPasswordAsync error");
                return Result.Fail("An unexpected error occurred while resetting password.");
            }
        }

        // 5. Update profile (first login profile completion)
        public async Task<Result<ProfileUpdateDto>> UpdateProfileAsync(string userId, ProfileUpdateDto dto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Result.Fail("User not found.");
                }
                    

                user.FirstName = dto.FirstName?.Trim();
                user.LastName = dto.LastName?.Trim();
                user.DateOfBirth = dto.DateOfBirth;
                user.PhoneNumber = dto.PhoneNumber;
                user.IsAccountConfigured = dto.MarkAccountConfigured;
                user.UpdatedAt = DateTime.UtcNow;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(';', updateResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("UpdateProfileAsync failed for user {UserId}: {Errors}", userId, errors);
                    return Result.Fail(errors);
                }

                return Result.Ok<ProfileUpdateDto>(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateProfileAsync error");
                return Result.Fail("An unexpected error occurred while updating profile.");
            }
        }

        // 6. Authenticator setup: reset/generate key and return provisioning URI
        public async Task<Result<AuthenticatorSetupDto>> GenerateAuthenticatorSetupAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Result.Fail<AuthenticatorSetupDto>("User not found.");
                }
                    

                // Reset authenticator key (so QR code is generated fresh)
                await _userManager.ResetAuthenticatorKeyAsync(user);
                var key = await _userManager.GetAuthenticatorKeyAsync(user);

                // Build otpauth URI
                var issuer = _configuration["Identity:Issuer"] ?? _configuration["AppSettings:AppName"] ?? "AHIS";
                var email = user.Email ?? user.UserName;
                var uri = $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(email)}?secret={key}&issuer={Uri.EscapeDataString(issuer)}&digits=6";

                // Save values (do not enable 2FA yet)
                user.AuthenticatorKey = key;
                user.AuthenticatorUri = uri;
                await _userManager.UpdateAsync(user);

                return Result.Ok(new AuthenticatorSetupDto { Key = key, ProvisionUri = uri });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GenerateAuthenticatorSetupAsync error");
                return Result.Fail<AuthenticatorSetupDto>("Failed to generate authenticator setup.");
            }
        }

        // Enable authenticator after user verifies a code
        public async Task<Result<IEnumerable<string>>> EnableAuthenticatorAsync(string userId, string verificationCode)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result.Fail<IEnumerable<string>>("User not found.");

                // Verify token
                var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);
                if (!isValid)
                    return Result.Fail<IEnumerable<string>>("Invalid verification code.");

                // Enable two factor for user
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                user.TwoFactorEnabledAt = DateTime.UtcNow;

                // Generate recovery codes
                var recovery = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                var codes = recovery.ToArray();

                // Persist recovery codes securely (we store as JSON string here). In production consider encrypting.
                user.RecoveryCodes = System.Text.Json.JsonSerializer.Serialize(codes);
                await _userManager.UpdateAsync(user);

                return Result.Ok(codes.AsEnumerable());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EnableAuthenticatorAsync error");
                return Result.Fail<IEnumerable<string>>("Failed to enable authenticator.");
            }
        }

        public async Task<Result> DisableAuthenticatorAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Result.Fail("User not found.");

                await _userManager.SetTwoFactorEnabledAsync(user, false);
                user.AuthenticatorKey = null;
                user.AuthenticatorUri = null;
                user.RecoveryCodes = null;
                user.TwoFactorEnabledAt = null;

                await _userManager.UpdateAsync(user);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DisableAuthenticatorAsync error");
                return Result.Fail("Failed to disable authenticator.");
            }
        }

        #region Helpers

        private string BuildCallbackUrl(string baseUrl, string path, IDictionary<string, string> query)
        {
            if (baseUrl.EndsWith('/')) baseUrl = baseUrl.TrimEnd('/');
            var sb = new StringBuilder();
            sb.Append(baseUrl);
            if (!path.StartsWith('/')) sb.Append('/');
            sb.Append(path);

            if (query != null && query.Count > 0)
            {
                sb.Append('?');
                sb.Append(string.Join('&', query.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}")));
            }

            return sb.ToString();
        }

        #endregion
    }
}
