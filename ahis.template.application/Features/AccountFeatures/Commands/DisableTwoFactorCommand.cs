using ahis.template.application.Shared.Mediator;
using ahis.template.identity.Interfaces;
using FluentResults;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace ahis.template.application.Features.AccountFeatures.Commands
{
    public class DisableTwoFactorCommand: IRequest<Result>
    {
    }

    public class DisableTwoFactorCommandHandler : IRequestHandler<DisableTwoFactorCommand, Result>
    {
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DisableTwoFactorCommandHandler(IAccountService accountService, IHttpContextAccessor httpContextAccessor)
        {
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result> Handle(DisableTwoFactorCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Result.Fail("Unauthorized");

            return await _accountService.DisableAuthenticatorAsync(userId);
        }
    }
}
