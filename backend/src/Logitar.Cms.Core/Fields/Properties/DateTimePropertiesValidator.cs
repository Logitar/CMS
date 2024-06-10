using FluentValidation;
using Logitar.Cms.Contracts.Fields.Properties;

namespace Logitar.Cms.Core.Fields.Properties;

public class DateTimePropertiesValidator : AbstractValidator<IDateTimeProperties>
{
  public DateTimePropertiesValidator()
  {
    RuleFor(x => x.MinimumValue).LessThanOrEqualTo(x => x.MaximumValue ?? DateTime.MaxValue);
  }
}
