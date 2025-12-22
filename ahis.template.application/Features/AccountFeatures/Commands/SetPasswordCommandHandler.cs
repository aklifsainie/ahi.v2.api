using ahis.template.application.Shared.Mediator;
using ahis.template.identity.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ahis.template.application.Features.AccountFeatures.Commands
{
    public class SetPasswordCommandHandler : IRequestHandler<SetPasswordCommand, Result>
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<SetPasswordCommandHandler> _logger;

        public SetPasswordCommandHandler(IAccountService accountService, ILogger<SetPasswordCommandHandler> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public async Task<Result> Handle(SetPasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Setting password for user {UserId}", request.UserId);
            return await _accountService.SetPasswordFirstTimeAsync(request.UserId, request.Password);
        }
    }
}
