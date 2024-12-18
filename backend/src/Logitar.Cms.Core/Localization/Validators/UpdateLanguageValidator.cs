using FluentValidation;
using Logitar.Cms.Core.Localization.Models;

namespace Logitar.Cms.Core.Localization.Validators;

internal class UpdateLanguageValidator : AbstractValidator<UpdateLanguagePayload>
{
  public UpdateLanguageValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Locale), () => RuleFor(x => x.Locale!).Locale());
  }
}
