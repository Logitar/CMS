using FluentValidation;
using Logitar.Cms.Contracts.Sessions;

namespace Logitar.Cms.Core.Sessions.Validators;

internal class RenewSessionValidator : AbstractValidator<RenewSessionPayload>
{
  public RenewSessionValidator()
  {
    RuleFor(x => x.RefreshToken).NotEmpty();
  }
}
