using FluentValidation.Results;
using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Validators;

internal class StringValueValidator : IFieldValueValidator
{
  private readonly IStringSettings _settings;

  public StringValueValidator(IStringSettings settings)
  {
    _settings = settings;
  }

  public Task<ValidationResult> ValidateAsync(string value, string propertyName, CancellationToken cancellationToken)
  {
    List<ValidationFailure> failures = new(capacity: 3);

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
    if (_settings.Pattern != null && !Regex.IsMatch(value, _settings.Pattern))
    {
      ValidationFailure failure = new(propertyName, $"The value must match the pattern '{_settings.Pattern}'.", value)
      {
        CustomState = new { _settings.Pattern },
        ErrorCode = "RegularExpressionValidator"
      };
      failures.Add(failure);
    }

    return Task.FromResult(new ValidationResult(failures));
  }
}
