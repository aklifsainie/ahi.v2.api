using ahis.template.application.Shared.Mediator;
using ahis.template.identity.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ahis.template.application.Features.AccountFeatures.Commands
{
    public class GenerateAuthenticatorSetupCommandHandler : IRequestHandler<GenerateAuthenticatorSetupCommand, Result<AuthenticatorSetupResult>>
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<GenerateAuthenticatorSetupCommandHandler> _logger;

        public GenerateAuthenticatorSetupCommandHandler(IAccountService accountService, ILogger<GenerateAuthenticatorSetupCommandHandler> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public async Task<Result<AuthenticatorSetupResult>> Handle(GenerateAuthenticatorSetupCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Generating authenticator setup for {UserId}", request.UserId);
            var res = await _accountService.GenerateAuthenticatorSetupAsync(request.UserId);
            if (!res.IsSuccess)
                return Result.Fail<AuthenticatorSetupResult>(res.Errors.FirstOrDefault()?.Message ?? "Failed to generate authenticator setup.");

            return Result.Ok(new AuthenticatorSetupResult { Key = res.Value?.Key, ProvisionUri = res.Value?.ProvisionUri });
        }
    }
}
