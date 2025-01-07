using FluentValidation.Results;
using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Validators;

internal class NumberValueValidator : IFieldValueValidator
{
  private readonly INumberSettings _settings;

  public NumberValueValidator(INumberSettings settings)
  {
    _settings = settings;
  }

  public ValidationResult Validate(string value, string propertyName)
  {
    List<ValidationFailure> failures = new(capacity: 2);

    if (double.TryParse(value, out double number))
    {
      if (number < _settings.MinimumValue)
      {
        ValidationFailure failure = new(propertyName, "TODO", value)
        {
          ErrorCode = "TODO"
          // TODO(fpion): CustomState?
        };
        failures.Add(failure);
      }
      if (number > _settings.MaximumValue)
      {
        ValidationFailure failure = new(propertyName, "TODO", value)
        {
          ErrorCode = "TODO"
          // TODO(fpion): CustomState?
        };
        failures.Add(failure);
      }
    }
    else
    {
      ValidationFailure failure = new(propertyName, "The value is not a valid number.", value)
      {
        ErrorCode = nameof(NumberValueValidator)
      };
      failures.Add(failure);
    }

    return new ValidationResult(failures);
  }
}
