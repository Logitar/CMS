using FluentValidation.Results;
using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Validators;

internal class SelectValueValidator : IFieldValueValidator
{
  private readonly ISelectSettings _settings;

  public SelectValueValidator(ISelectSettings settings)
  {
    _settings = settings;
  }

  public ValidationResult Validate(string value, string propertyName)
  {
    List<ValidationFailure> failures = new(capacity: -1); // TODO(fpion): capacity

    //if (value.Length < _settings.MinimumLength)
    //{
    //  ValidationFailure failure = new(propertyName, $"The length of the value must be at least {_settings.MinimumLength} characters.", value)
    //  {
    //    CustomState = new { _settings.MinimumLength },
    //    ErrorCode = "MinimumLengthValidator"
    //  };
    //  failures.Add(failure);
    //}
    //if (value.Length > _settings.MaximumLength)
    //{
    //  ValidationFailure failure = new(propertyName, $"The length of the value may not exceed {_settings.MaximumLength} characters.", value)
    //  {
    //    CustomState = new { _settings.MaximumLength },
    //    ErrorCode = "MaximumLengthValidator"
    //  };
    //  failures.Add(failure);
    //}
    //if (_settings.Pattern != null && !Regex.IsMatch(value, _settings.Pattern))
    //{
    //  ValidationFailure failure = new(propertyName, $"The value must match the pattern '{_settings.Pattern}'.", value)
    //  {
    //    CustomState = new { _settings.Pattern },
    //    ErrorCode = "RegularExpressionValidator"
    //  };
    //  failures.Add(failure);
    //} // TODO(fpion): implement

    return new ValidationResult(failures);
  }
}
