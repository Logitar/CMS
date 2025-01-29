using FluentValidation;
using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Validators;

public class DateTimeSettingsValidator : AbstractValidator<IDateTimeSettings>
{
  public DateTimeSettingsValidator()
  {
    When(x => x.MinimumValue.HasValue && x.MaximumValue.HasValue, () =>
    {
      RuleFor(x => x.MinimumValue!.Value).LessThanOrEqualTo(x => x.MaximumValue!.Value);
      RuleFor(x => x.MaximumValue!.Value).GreaterThanOrEqualTo(x => x.MinimumValue!.Value);
    });
  }
}
