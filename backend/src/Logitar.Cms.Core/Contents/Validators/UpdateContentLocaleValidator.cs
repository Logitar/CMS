using FluentValidation;
using Logitar.Cms.Contracts.Contents;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Contents.Validators;

internal class UpdateContentLocaleValidator : AbstractValidator<UpdateContentLocalePayload>
{
  public UpdateContentLocaleValidator(IUniqueNameSettings uniqueNameSettings)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).SetValidator(new UniqueNameValidator(uniqueNameSettings)));
  }
}
