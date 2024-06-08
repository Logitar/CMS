using FluentValidation;
using Logitar.Cms.Contracts.Fields.Properties;

namespace Logitar.Cms.Core.Fields.Properties;

public class StringPropertiesValidator : AbstractValidator<IStringProperties>
{
  public StringPropertiesValidator()
  {
    RuleFor(x => x.MinimumLength).GreaterThanOrEqualTo(0).LessThanOrEqualTo(x => x.MaximumLength);
    RuleFor(x => x.MaximumLength).GreaterThan(0);
    When(x => x.Pattern != null, () => RuleFor(x => x.Pattern).NotEmpty());
  }
}
