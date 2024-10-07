using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Core.FieldTypes.Validators;

public class StringFieldValueValidator : AbstractValidator<string>
{
  public StringFieldValueValidator(IStringProperties properties)
  {
    if (properties.MinimumLength.HasValue)
    {
      RuleFor(x => x).MinimumLength(properties.MinimumLength.Value);
    }
    if (properties.MaximumLength.HasValue)
    {
      RuleFor(x => x).MaximumLength(properties.MaximumLength.Value);
    }

    if (properties.Pattern != null)
    {
      RuleFor(x => x).Matches(properties.Pattern);
    }
  }
}
