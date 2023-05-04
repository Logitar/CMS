using FluentValidation;
using Logitar.Cms.Core.Users.Events;

namespace Logitar.Cms.Core.Users.Validators;

internal class EmailChangedValidator : AbstractValidator<EmailChanged>
{
  public EmailChangedValidator()
  {
    When(x => x.Email == null, () => RuleFor(x => x.VerificationAction).NotEqual(VerificationAction.Verify))
      .Otherwise(() => RuleFor(x => x.Email!).SetValidator(new ReadOnlyEmailValidator()));
  }
}
