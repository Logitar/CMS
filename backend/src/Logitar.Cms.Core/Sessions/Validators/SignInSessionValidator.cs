using FluentValidation;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Validators;

namespace Logitar.Cms.Core.Sessions.Validators;

public class SignInSessionValidator : AbstractValidator<SignInSessionPayload>
{
  public SignInSessionValidator()
  {
    RuleFor(x => x.Username).NotEmpty();
    RuleFor(x => x.Password).NotEmpty();

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeContractValidator());
  }
}
