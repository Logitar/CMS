using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Core.FieldTypes.Validators;

public class DateTimeFieldValueValidator : AbstractValidator<DateTime>
{
  public DateTimeFieldValueValidator(IDateTimeProperties properties)
  {
    if (properties.MinimumValue.HasValue)
    {
      RuleFor(x => x).GreaterThanOrEqualTo(properties.MinimumValue.Value);
    }
    if (properties.MaximumValue.HasValue)
    {
      RuleFor(x => x).LessThanOrEqualTo(properties.MaximumValue.Value);
    }
  }
}
