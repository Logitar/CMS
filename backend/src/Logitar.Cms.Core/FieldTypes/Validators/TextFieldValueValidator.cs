using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Core.FieldTypes.Validators;

public class TextFieldValueValidator : AbstractValidator<string>
{
  public TextFieldValueValidator(ITextProperties properties)
  {
    if (properties.MinimumLength.HasValue)
    {
      RuleFor(x => x).MinimumLength(properties.MinimumLength.Value);
    }
    if (properties.MaximumLength.HasValue)
    {
      RuleFor(x => x).MaximumLength(properties.MaximumLength.Value);
    }
  }
}
