using FluentValidation;
using Logitar.Cms.Contracts.Languages;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Languages.Validators;

public class CreateLanguageValidator : AbstractValidator<CreateLanguagePayload>
{
  public CreateLanguageValidator()
  {
    RuleFor(x => x.Locale).SetValidator(new LocaleValidator());
  }
}
