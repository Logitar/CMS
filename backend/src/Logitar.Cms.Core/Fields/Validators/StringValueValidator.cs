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

  public ValidationResult Validate(string value, string propertyName)
  {
    List<ValidationFailure> failures = new(capacity: 2);

    if (value.Length < _settings.MinimumLength)
    {
      ValidationFailure failure = new(propertyName, "TODO", value)
      {
        ErrorCode = "TODO"
        // TODO(fpion): CustomState?
      };
      failures.Add(failure);
    }
    if (value.Length > _settings.MaximumLength)
    {
      ValidationFailure failure = new(propertyName, "TODO", value)
      {
        ErrorCode = "TODO"
        // TODO(fpion): CustomState?
      };
      failures.Add(failure);
    }
    if (_settings.Pattern != null && !Regex.IsMatch(value, _settings.Pattern))
    {
      ValidationFailure failure = new(propertyName, "TODO", value)
      {
        ErrorCode = "TODO"
        // TODO(fpion): CustomState?
      };
      failures.Add(failure);
    }

    return new ValidationResult(failures);
  }
}
