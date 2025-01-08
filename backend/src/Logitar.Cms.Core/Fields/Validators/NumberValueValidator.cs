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

  public Task<ValidationResult> ValidateAsync(string value, string propertyName, CancellationToken cancellationToken)
  {
    List<ValidationFailure> failures = new(capacity: 2);

    if (double.TryParse(value, out double number))
    {
      if (number < _settings.MinimumValue)
      {
        ValidationFailure failure = new(propertyName, $"The value must be greater than or equal to {_settings.MinimumValue}.", value)
        {
          CustomState = new { _settings.MinimumValue },
          ErrorCode = "GreaterThanOrEqualValidator"
        };
        failures.Add(failure);
      }
      if (number > _settings.MaximumValue)
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
      ValidationFailure failure = new(propertyName, "The value is not a valid number.", value)
      {
        ErrorCode = nameof(NumberValueValidator)
      };
      failures.Add(failure);
    }

    return Task.FromResult(new ValidationResult(failures));
  }
}
