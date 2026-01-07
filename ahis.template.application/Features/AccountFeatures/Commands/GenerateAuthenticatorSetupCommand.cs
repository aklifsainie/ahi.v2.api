using ahis.template.application.Shared.Mediator;
using ahis.template.domain.Models.ViewModels.AccountVM;
using ahis.template.identity.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace ahis.template.application.Features.AccountFeatures.Commands
{
    public class GenerateAuthenticatorSetupCommand : IRequest<Result<AuthenticatorSetupResponseVM>>
    {
        [Required]
        public string UserId { get; set; } = null!;
    }

    public class GenerateAuthenticatorSetupCommandHandler : IRequestHandler<GenerateAuthenticatorSetupCommand, Result<AuthenticatorSetupResponseVM>>
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<GenerateAuthenticatorSetupCommandHandler> _logger;

        public GenerateAuthenticatorSetupCommandHandler(IAccountService accountService, ILogger<GenerateAuthenticatorSetupCommandHandler> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public async Task<Result<AuthenticatorSetupResponseVM>> Handle(GenerateAuthenticatorSetupCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Generating authenticator setup for {UserId}", request.UserId);

            var result = await _accountService.GenerateAuthenticatorSetupAsync(request.UserId);
            if (!result.IsSuccess)
            {
                return Result.Fail<AuthenticatorSetupResponseVM>(result.Errors.FirstOrDefault()?.Message ?? "Failed to generate authenticator setup.");
            }

            AuthenticatorSetupResponseVM vm = new AuthenticatorSetupResponseVM
            { 
                Key = result.Value.Key,
                ProvisionUri = result.Value.ProvisionUri
            };

            return Result.Ok<AuthenticatorSetupResponseVM>(vm);
        }
    }
}
