using ahis.template.application.Shared.Mediator;
using FluentResults;
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
}
