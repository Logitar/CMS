using FluentValidation;
using Logitar.Cms.Contracts.Contents;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Contents.Validators;

public class CreateContentValidator : AbstractValidator<CreateContentPayload>
{
  public CreateContentValidator(bool isInvariant, IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.ContentTypeId).NotEmpty();

    if (isInvariant)
    {
      RuleFor(x => x.LanguageId).Null().WithMessage("'{PropertyName}' must be null when the content type is invariant.");
    }
    else
    {
      RuleFor(x => x.LanguageId).NotEmpty().WithMessage("'{PropertyName}' is required when the content type is not invariant.");
    }

    RuleFor(x => x.UniqueName).SetValidator(new UniqueNameValidator(uniqueNameSettings));
  }
}
