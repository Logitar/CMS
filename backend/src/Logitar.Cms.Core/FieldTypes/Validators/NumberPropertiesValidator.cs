using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Core.FieldTypes.Validators;

public class NumberPropertiesValidator : AbstractValidator<INumberProperties>
{
  public NumberPropertiesValidator()
  {
    When(x => x.MinimumValue.HasValue && x.MaximumValue.HasValue, () =>
    {
      RuleFor(x => x.MinimumValue!.Value).LessThanOrEqualTo(x => x.MaximumValue!.Value);
      RuleFor(x => x.MaximumValue!.Value).GreaterThanOrEqualTo(x => x.MinimumValue!.Value);
    });

    // ISSUE: https://github.com/Logitar/CMS/issues/28
  }
}
