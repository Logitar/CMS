using FluentValidation;
using Logitar.Cms.Contracts.Fields.Properties;

namespace Logitar.Cms.Core.Fields.Properties;

public class NumberPropertiesValidator : AbstractValidator<INumberProperties>
{
  public NumberPropertiesValidator()
  {
    RuleFor(x => x.MinimumValue).LessThanOrEqualTo(x => x.MaximumValue);
    RuleFor(x => x.Step).GreaterThan(0).LessThanOrEqualTo(x => x.MaximumValue);
  }
}
