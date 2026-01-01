using ahis.template.application.Shared.Mediator;
using ahis.template.domain.Models.ViewModels.AuthenticationVM;
using ahis.template.identity.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ahis.template.application.Features.AuthenticationFeatures.Commands
{
    /// <summary>
    /// Verify two-factor authentication code and complete login
    /// </summary>
    public class VerifyTwoFactorLoginCommand : IRequest<Result<AuthenticationResponseVM>>
    {
        public string UserId { get; set; }
        /// <summary>
        /// 6-digit code from authenticator app
        /// </summary>
        public string Code { get; set; } = default!;

        /// <summary>
        /// Remember this device (skip 2FA next time)
        /// </summary>
        public bool RememberMachine { get; set; } = false;
    }


    public class VerifyTwoFactorLoginCommandHandler: IRequestHandler<VerifyTwoFactorLoginCommand, Result<AuthenticationResponseVM>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<VerifyTwoFactorLoginCommandHandler> _logger;

        public VerifyTwoFactorLoginCommandHandler(IAuthenticationService authenticationService, ILogger<VerifyTwoFactorLoginCommandHandler> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        public async Task<Result<AuthenticationResponseVM>> Handle(VerifyTwoFactorLoginCommand request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(request.UserId))
                return Result.Fail<AuthenticationResponseVM>("UserId is required.");

            if (string.IsNullOrWhiteSpace(request.Code))
                return Result.Fail<AuthenticationResponseVM>("Verification code is required.");

            try
            {
                return await _authenticationService.VerifyTwoFactorAsync(
                    request.UserId,
                    code: request.Code,
                    rememberMachine: request.RememberMachine
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "2FA verification failed for user {UserId}",
                    request.UserId);

                return Result.Fail<AuthenticationResponseVM>(
                    "Failed to verify two-factor authentication.");
            }
        }
    }
}
