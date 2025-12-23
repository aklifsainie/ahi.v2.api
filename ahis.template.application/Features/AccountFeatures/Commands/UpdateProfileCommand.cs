using ahis.template.application.Shared.Mediator;
using ahis.template.identity.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace ahis.template.application.Features.AccountFeatures.Commands
{
    public class UpdateProfileCommand : IRequest<Result<object>>
    {
        [Required]
        public string UserId { get; set; } = null!;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public bool MarkAccountConfigured { get; set; } = true;
    }


    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<object>>
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<UpdateProfileCommandHandler> _logger;

        public UpdateProfileCommandHandler(IAccountService accountService, ILogger<UpdateProfileCommandHandler> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public async Task<Result<object>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating profile for {UserId}", request.UserId);
            var dto = new identity.Services.ProfileUpdateDto
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber,
                MarkAccountConfigured = request.MarkAccountConfigured
            };

            return await _accountService.UpdateProfileAsync(request.UserId, dto);
        }
    }
}
