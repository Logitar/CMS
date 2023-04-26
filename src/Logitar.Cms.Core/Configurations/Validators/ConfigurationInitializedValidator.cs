using FluentValidation;
using Logitar.Cms.Core.Configurations.Events;

namespace Logitar.Cms.Core.Configurations.Validators;

internal class ConfigurationInitializedValidator : AbstractValidator<ConfigurationInitialized>
{
  public ConfigurationInitializedValidator()
  {
    RuleFor(x => x.Secret).NotEmpty()
      .MinimumLength(256 / 8)
      .MaximumLength(512 / 8);

    RuleFor(x => x.LoggingSettings).NotNull()
      .SetValidator(new ReadOnlyLoggingSettingsValidator());

    RuleFor(x => x.UsernameSettings).NotNull()
      .SetValidator(new ReadOnlyUsernameSettingsValidator());

    RuleFor(x => x.PasswordSettings).NotNull()
      .SetValidator(new ReadOnlyPasswordSettingsValidator());
  }
}
