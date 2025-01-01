using FluentValidation;
using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Validators;

public class NumberSettingsValidator : AbstractValidator<INumberSettings>
{
  public NumberSettingsValidator()
  {
    When(x => x.MinimumValue.HasValue && x.MaximumValue.HasValue, () =>
    {
      RuleFor(x => x.MinimumValue!.Value).LessThan(x => x.MaximumValue!.Value);
      RuleFor(x => x.MaximumValue!.Value).GreaterThan(x => x.MinimumValue!.Value);
    });

    When(x => x.MinimumValue.HasValue && x.Step.HasValue, () =>
    {
      RuleFor(x => x.MinimumValue!.Value).LessThan(x => x.Step!.Value);
      RuleFor(x => x.Step!.Value).GreaterThan(x => x.MinimumValue!.Value);
    });

    When(x => x.MaximumValue.HasValue && x.Step.HasValue, () =>
    {
      RuleFor(x => x.Step!.Value).LessThan(x => x.MaximumValue!.Value);
      RuleFor(x => x.MaximumValue!.Value).GreaterThan(x => x.Step!.Value);
    });
  }
}
