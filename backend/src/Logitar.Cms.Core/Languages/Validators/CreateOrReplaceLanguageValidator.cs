using FluentValidation;
using Logitar.Cms.Contracts.Languages;

namespace Logitar.Cms.Core.Languages.Validators;

internal class CreateOrReplaceLanguageValidator : AbstractValidator<CreateOrReplaceLanguagePayload>
{
  public CreateOrReplaceLanguageValidator()
  {
    RuleFor(x => x.Locale).Locale();
  }
}
