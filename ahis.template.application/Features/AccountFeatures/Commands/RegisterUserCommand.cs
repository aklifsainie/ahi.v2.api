using ahis.template.application.Shared.Mediator;
using ahis.template.identity.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace ahis.template.application.Features.AccountFeatures.Commands
{
    public class RegisterUserCommand : IRequest<Result<object>>
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string UserName { get; set; } = null!;

        // Client should provide a base url for email callback (frontend)
        [Required]
        public string CallbackBaseUrl { get; set; } = null!;
    }


    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<object>>
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(IAccountService accountService, ILogger<RegisterUserCommandHandler> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public async Task<Result<object>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registering user {Email}", request.Email);
            var res = await _accountService.RegisterAsync(request.Email, request.UserName, request.CallbackBaseUrl);
            if (!res.IsSuccess)
                return Result.Fail<object>(res.Errors.FirstOrDefault()?.Message ?? "Registration failed.");

            return Result.Ok<object>(res.Value).WithSuccess($"Username {request.UserName} has been register");
        }
    }
}
