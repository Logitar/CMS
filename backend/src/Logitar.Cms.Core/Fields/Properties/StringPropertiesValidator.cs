using FluentValidation;
using Logitar.Cms.Contracts.Fields.Properties;

namespace Logitar.Cms.Core.Fields.Properties;

public class StringPropertiesValidator : AbstractValidator<IStringProperties>
{
  public StringPropertiesValidator()
  {
    RuleFor(x => x.MinimumLength).GreaterThanOrEqualTo(0).LessThanOrEqualTo(x => x.MinimumLength);
    RuleFor(x => x.MaximumLength).GreaterThanOrEqualTo(0);
    When(x => x.Pattern != null, () => RuleFor(x => x.Pattern).NotEmpty());
  }
}
