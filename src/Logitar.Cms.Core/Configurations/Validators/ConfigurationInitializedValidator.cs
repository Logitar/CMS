using FluentValidation;
using Logitar.Cms.Core.Configurations.Events;

namespace Logitar.Cms.Core.Configurations.Validators;

internal class ConfigurationInitializedValidator : AbstractValidator<ConfigurationInitialized>
{
  private const int MinimumSecretLength = 256 / 8; // JSON Web Tokens symmetric signing keys should use at least 256 bits. Reference: https://web-token.spomky-labs.com/advanced-topics-1/security-recommendations
  private const int MaximumSecretLength = 512 / 8; // JSON Web Tokens symmetric signing keys should not be too long. We will be using the HS256 algorithm for now.

  public ConfigurationInitializedValidator()
  {
    RuleFor(x => x.Secret).NotEmpty()
      .MinimumLength(MinimumSecretLength)
      .MaximumLength(MaximumSecretLength);

    RuleFor(x => x.LoggingSettings).NotNull()
      .SetValidator(new ReadOnlyLoggingSettingsValidator());

    RuleFor(x => x.UsernameSettings).NotNull()
      .SetValidator(new ReadOnlyUsernameSettingsValidator());

    RuleFor(x => x.PasswordSettings).NotNull()
      .SetValidator(new ReadOnlyPasswordSettingsValidator());
  }
}
