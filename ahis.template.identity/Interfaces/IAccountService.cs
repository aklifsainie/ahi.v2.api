using ahis.template.identity.Models;
using ahis.template.identity.Services;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ahis.template.identity.Interfaces
{
    public interface IAccountService
    {
        Task<Result<string>> RegisterAsync(string email, string userName, string callbackBaseUrl);
        Task<Result> SendEmailConfirmationAsync(ApplicationUser user, string callbackBaseUrl);
        Task<Result> ConfirmEmailAsync(string userId, string encodedToken);
        Task<Result> SetPasswordFirstTimeAsync(string userId, string password);
        Task<Result<string>> GeneratePasswordResetTokenAsync(string email);
        Task<Result> ResetPasswordAsync(string userId, string encodedToken, string newPassword);
        Task<Result> UpdateProfileAsync(string userId, ProfileUpdateDto dto);
        Task<Result<AuthenticatorSetupDto>> GenerateAuthenticatorSetupAsync(string userId);
        Task<Result<IEnumerable<string>>> EnableAuthenticatorAsync(string userId, string verificationCode);
        Task<Result> DisableAuthenticatorAsync(string userId);
    }
}
