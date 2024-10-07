using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Core.FieldTypes.Validators;

public class NumberFieldValueValidator : AbstractValidator<double>
{
  public NumberFieldValueValidator(INumberProperties properties)
  {
    if (properties.MinimumValue.HasValue)
    {
      RuleFor(x => x).GreaterThanOrEqualTo(properties.MinimumValue.Value);
    }
    if (properties.MaximumValue.HasValue)
    {
      RuleFor(x => x).LessThanOrEqualTo(properties.MaximumValue.Value);
    }

    // ISSUE: https://github.com/Logitar/CMS/issues/28
  }
}
