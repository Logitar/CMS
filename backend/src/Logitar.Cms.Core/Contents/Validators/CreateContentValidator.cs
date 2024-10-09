using FluentValidation;
using Logitar.Cms.Contracts.Contents;
using ContentType = Logitar.Cms.Core.ContentTypes.ContentType;

namespace Logitar.Cms.Core.Contents.Validators;

internal class CreateContentValidator : AbstractValidator<CreateContentPayload>
{
  public CreateContentValidator(ContentType contentType)
  {
    if (contentType.IsInvariant)
    {
      RuleFor(x => x.LanguageId).Null().WithMessage("'{PropertyName}' must be null when the content type is invariant.");
    }
    else
    {
      RuleFor(x => x.LanguageId).NotEmpty().WithMessage("'{PropertyName}' is required when the content type is not invariant.");
    }

    RuleFor(x => x.UniqueName).UniqueName(Content.UniqueNameSettings);
  }
}
