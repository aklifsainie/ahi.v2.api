using ahis.template.application.Shared.Mediator;
using FluentResults;
using System.ComponentModel.DataAnnotations;

namespace ahis.template.application.Features.AccountFeatures.Commands
{
    public class GenerateAuthenticatorSetupCommand : IRequest<Result<AuthenticatorSetupResult>>
    {
        [Required]
        public string UserId { get; set; } = null!;
    }

    public class AuthenticatorSetupResult
    {
        public string? Key { get; set; }
        public string? ProvisionUri { get; set; }
    }
}
