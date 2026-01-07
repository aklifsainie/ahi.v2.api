using ahis.template.application.Shared.Mediator;
using ahis.template.identity.Interfaces;
using ahis.template.identity.Models.DTOs;
using FluentResults;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace ahis.template.application.Features.AccountFeatures.Commands
{
    public class UpdateProfileCommand : IRequest<Result<ProfileUpdateDto>>
    {
        [Required]
        public string UserId { get; set; } = null!;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
    }


    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<ProfileUpdateDto>>
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<UpdateProfileCommandHandler> _logger;

        public UpdateProfileCommandHandler(IAccountService accountService, ILogger<UpdateProfileCommandHandler> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public async Task<Result<ProfileUpdateDto>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating profile for {UserId}", request.UserId);
            var dto = new ProfileUpdateDto
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber,
                MarkAccountConfigured = true
            };

            var result = await _accountService.UpdateProfileAsync(request.UserId, dto);

            if (result.IsFailed)
            {
                _logger.LogWarning("Failed to update profile for {UserId}: {Errors}", request.UserId, result.Errors);
                return Result.Fail<ProfileUpdateDto>(result.Errors);
            }

            return Result.Ok<ProfileUpdateDto>(dto).WithSuccess("Profile updated");
        }
    }
}
