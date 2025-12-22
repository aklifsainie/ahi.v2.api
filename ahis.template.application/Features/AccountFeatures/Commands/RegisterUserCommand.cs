using ahis.template.application.Shared.Mediator;
using FluentResults;
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
}
