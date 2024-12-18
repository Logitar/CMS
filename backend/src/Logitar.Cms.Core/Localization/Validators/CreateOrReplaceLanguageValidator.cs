using FluentValidation;
using Logitar.Cms.Core.Localization.Models;

namespace Logitar.Cms.Core.Localization.Validators;

internal class CreateOrReplaceLanguageValidator : AbstractValidator<CreateOrReplaceLanguagePayload>
{
  public CreateOrReplaceLanguageValidator()
  {
    RuleFor(x => x.Locale).Locale();
  }
}
