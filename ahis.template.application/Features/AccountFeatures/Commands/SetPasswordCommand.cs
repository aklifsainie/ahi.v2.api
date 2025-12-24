using ahis.template.application.Shared.Mediator;
using ahis.template.identity.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace ahis.template.application.Features.AccountFeatures.Commands
{
    public class SetPasswordCommand : IRequest<Result<object>>
    {
        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = null!;
    }


    public class SetPasswordCommandHandler : IRequestHandler<SetPasswordCommand, Result<object>>
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<SetPasswordCommandHandler> _logger;

        public SetPasswordCommandHandler(IAccountService accountService, ILogger<SetPasswordCommandHandler> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public async Task<Result<object>> Handle(SetPasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Setting password for user {UserId}", request.UserId);
            var response = await _accountService.SetPasswordFirstTimeAsync(request.UserId, request.Password);

            if (!response.IsSuccess)
                return Result.Fail<object>(response.Errors.FirstOrDefault()?.Message ?? "Registration failed.");


            return Result.Ok<object>(true).WithSuccess("Password has been saved");
        }
    }
}
