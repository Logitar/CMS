using FluentValidation;
using Logitar.Cms.Contracts.Languages;

namespace Logitar.Cms.Core.Languages.Validators;

internal class UpdateLanguageValidator : AbstractValidator<UpdateLanguagePayload>
{
  public UpdateLanguageValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Locale), () => RuleFor(x => x.Locale!).Locale());
  }
}
