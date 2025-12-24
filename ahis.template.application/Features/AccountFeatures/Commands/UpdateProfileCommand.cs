using ahis.template.application.Shared.Mediator;
using ahis.template.identity.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace ahis.template.application.Features.AccountFeatures.Commands
{
    public class UpdateProfileCommand : IRequest<Result<UpdateProfileCommand>>
    {
        [Required]
        public string UserId { get; set; } = null!;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
    }


    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<UpdateProfileCommand>>
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<UpdateProfileCommandHandler> _logger;

        public UpdateProfileCommandHandler(IAccountService accountService, ILogger<UpdateProfileCommandHandler> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public async Task<Result<UpdateProfileCommand>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating profile for {UserId}", request.UserId);
            var dto = new identity.Services.ProfileUpdateDto
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber,
                MarkAccountConfigured = true
            };

            var res = await _accountService.UpdateProfileAsync(request.UserId, dto);

            return Result.Ok<UpdateProfileCommand>(request).WithSuccess("Profile updated");
        }
    }
}
