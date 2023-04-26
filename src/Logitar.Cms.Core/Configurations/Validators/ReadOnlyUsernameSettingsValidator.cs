using FluentValidation;

namespace Logitar.Cms.Core.Configurations.Validators;

internal class ReadOnlyUsernameSettingsValidator : AbstractValidator<ReadOnlyUsernameSettings>
{
  public ReadOnlyUsernameSettingsValidator()
  {
    RuleFor(x => x.AllowedCharacters).NullOrNotEmpty();
  }
}
