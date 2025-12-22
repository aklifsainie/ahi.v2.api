using ahis.template.application.Shared.Mediator;
using FluentResults;
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
}
