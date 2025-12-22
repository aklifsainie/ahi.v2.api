using ahis.template.application.Shared.Mediator;
using ahis.template.identity.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ahis.template.application.Features.AccountFeatures.Commands
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<object>>
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<ConfirmEmailCommandHandler> _logger;

        public ConfirmEmailCommandHandler(IAccountService accountService, ILogger<ConfirmEmailCommandHandler> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public async Task<Result<object>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Confirming email for user {UserId}", request.UserId);
            var res = await _accountService.ConfirmEmailAsync(request.UserId, request.Token);
            if (!res.IsSuccess)
                return Result.Fail<object>(res.Errors.FirstOrDefault()?.Message ?? "Confirm email failed.");

            return Result.Ok<object>(null);
        }
    }
}
