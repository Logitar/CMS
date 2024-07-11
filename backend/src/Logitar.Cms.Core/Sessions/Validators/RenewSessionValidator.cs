using FluentValidation;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Validators;

namespace Logitar.Cms.Core.Sessions.Validators;

public class RenewSessionValidator : AbstractValidator<RenewSessionPayload>
{
  public RenewSessionValidator()
  {
    RuleFor(x => x.RefreshToken).NotEmpty();

    RuleForEach(x => x.CustomAttributes).SetValidator(new ContractCustomAttributeValidator());
  }
}
