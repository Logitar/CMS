using FluentValidation;
using Logitar.Cms.Contracts.Contents;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Contents.Validators;

internal class CreateContentValidator : AbstractValidator<CreateContentPayload>
{
  public CreateContentValidator(bool isInvariant, IUniqueNameSettings uniqueNameSettings)
  {
    if (isInvariant)
    {
      RuleFor(x => x.LanguageId).Null().WithMessage("'{PropertyName}' must be null when the content type is invariant.");
    }
    else
    {
      RuleFor(x => x.LanguageId).NotNull().WithMessage("'{PropertyName}' must be provided when the content type is not invariant.");
    }

    RuleFor(x => x.UniqueName).SetValidator(new UniqueNameValidator(uniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
  }
}
