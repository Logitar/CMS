using FluentValidation.Results;
using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Validators;

internal class RichTextValueValidator : IFieldValueValidator
{
  private readonly IRichTextSettings _settings;

  public RichTextValueValidator(IRichTextSettings settings)
  {
    _settings = settings;
  }

  public ValidationResult Validate(string value, string propertyName)
  {
    List<ValidationFailure> failures = new(capacity: 2);

    if (value.Length < _settings.MinimumLength)
    {
      ValidationFailure failure = new(propertyName, $"The length of the value must be at least {_settings.MinimumLength} characters.", value)
      {
        CustomState = new { _settings.MinimumLength },
        ErrorCode = "MinimumLengthValidator"
      };
      failures.Add(failure);
    }
    if (value.Length > _settings.MaximumLength)
    {
      ValidationFailure failure = new(propertyName, $"The length of the value may not exceed {_settings.MaximumLength} characters.", value)
      {
        CustomState = new { _settings.MaximumLength },
        ErrorCode = "MaximumLengthValidator"
      };
      failures.Add(failure);
    }

    return new ValidationResult(failures);
  }
}
