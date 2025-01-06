using FluentValidation;
using Logitar.Cms.Core.Sessions.Models;
using Logitar.Cms.Core.Validators;

namespace Logitar.Cms.Core.Sessions.Validators;

internal class RenewSessionValidator : AbstractValidator<RenewSessionPayload>
{
  public RenewSessionValidator()
  {
    RuleFor(x => x.RefreshToken).NotEmpty();

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
