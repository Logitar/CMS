using FluentValidation;
using Logitar.Cms.Core.Languages.Events;

namespace Logitar.Cms.Core.Languages.Validators;

internal class LanguageCreatedValidator : AbstractValidator<LanguageCreated>
{
  public LanguageCreatedValidator()
  {
    RuleFor(x => x.Locale).NotNull()
      .Locale();
  }
}
