using FluentValidation;
using Logitar.Cms.Contracts.Configurations;

namespace Logitar.Cms.Core.Configurations.Validators;

internal class LoggingSettingsValidator : AbstractValidator<ILoggingSettings>
{
  public LoggingSettingsValidator()
  {
    RuleFor(x => x.Extent).IsInEnum();
    When(x => x.Extent == LoggingExtent.None, () => RuleFor(x => x.OnlyErrors).Equal(false));
  }
}
