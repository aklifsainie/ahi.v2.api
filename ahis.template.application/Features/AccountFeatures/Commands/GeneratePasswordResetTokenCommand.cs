using ahis.template.application.Shared.Mediator;
using ahis.template.identity.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace ahis.template.application.Features.AccountFeatures.Commands
{
    public class GeneratePasswordResetTokenCommand : IRequest<Result<object>>
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
    }

    public class ResetPasswordCommand : IRequest<Result<object>>
    {
        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public string Token { get; set; } = null!;

        [Required]
        public string NewPassword { get; set; } = null!;
    }


    public class GeneratePasswordResetTokenCommandHandler : IRequestHandler<GeneratePasswordResetTokenCommand, Result<object>>
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<GeneratePasswordResetTokenCommandHandler> _logger;

        public GeneratePasswordResetTokenCommandHandler(IAccountService accountService, ILogger<GeneratePasswordResetTokenCommandHandler> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public async Task<Result<object>> Handle(GeneratePasswordResetTokenCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Generating password reset token for {Email}", request.Email);
            var res = await _accountService.GeneratePasswordResetTokenAsync(request.Email);
            if (!res.IsSuccess)
                return Result.Fail<object>(res.Errors.FirstOrDefault()?.Message ?? "Failed to generate reset token.");

            return Result.Ok((object)res.Value);
        }
    }
}
