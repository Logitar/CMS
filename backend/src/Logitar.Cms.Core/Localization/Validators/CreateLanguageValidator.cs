using FluentValidation;
using Logitar.Cms.Contracts.Localization;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Localization.Validators;

internal class CreateLanguageValidator : AbstractValidator<CreateLanguagePayload>
{
  public CreateLanguageValidator()
  {
    RuleFor(x => x.Locale).SetValidator(new LocaleValidator());
  }
}
