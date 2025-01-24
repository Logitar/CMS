using FluentValidation;
using Logitar.Cms.Core.Users.Models;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Users.Validators;

internal class ChangePasswordValidator : AbstractValidator<ChangePasswordPayload>
{
  public ChangePasswordValidator(IPasswordSettings passwordSettings)
  {
    When(x => x.Current != null, () => RuleFor(x => x.Current).NotEmpty());
    RuleFor(x => x.New).Password(passwordSettings);
  }
}
