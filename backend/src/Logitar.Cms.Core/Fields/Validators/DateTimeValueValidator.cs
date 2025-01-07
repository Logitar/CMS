using FluentValidation.Results;
using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Validators;

internal class DateTimeValueValidator : IFieldValueValidator
{
  private readonly IDateTimeSettings _settings;

  public DateTimeValueValidator(IDateTimeSettings settings)
  {
    _settings = settings;
  }

  public ValidationResult Validate(string value, string propertyName)
  {
    List<ValidationFailure> failures = new(capacity: 2);

    if (DateTime.TryParse(value, out DateTime dateTime))
    {
      if (dateTime < _settings.MinimumValue)
      {
        ValidationFailure failure = new(propertyName, $"The value must be greater than or equal to {_settings.MinimumValue}.", value)
        {
          CustomState = new { _settings.MinimumValue },
          ErrorCode = "GreaterThanOrEqualValidator"
        };
        failures.Add(failure);
      }
      if (dateTime > _settings.MaximumValue)
      {
        ValidationFailure failure = new(propertyName, $"The value must be less than or equal to {_settings.MaximumValue}.", value)
        {
          CustomState = new { _settings.MaximumValue },
          ErrorCode = "LessThanOrEqualValidator"
        };
        failures.Add(failure);
      }
    }
    else
    {
      ValidationFailure failure = new(propertyName, "The value is not a valid DateTime.", value)
      {
        ErrorCode = nameof(DateTimeValueValidator)
      };
      failures.Add(failure);
    }

    return new ValidationResult(failures);
  }
}
