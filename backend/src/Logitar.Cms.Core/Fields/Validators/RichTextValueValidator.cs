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

    return new ValidationResult(failures);
  }
}
