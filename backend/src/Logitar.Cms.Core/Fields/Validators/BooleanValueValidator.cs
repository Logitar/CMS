using FluentValidation.Results;

namespace Logitar.Cms.Core.Fields.Validators;

internal class BooleanValueValidator : IFieldValueValidator
{
  public ValidationResult Validate(string value, string propertyName)
  {
    List<ValidationFailure> failures = new(capacity: 1);

    if (!bool.TryParse(value, out _))
    {
      ValidationFailure failure = new(propertyName, "The value is not a valid boolean.", value)
      {
        ErrorCode = nameof(BooleanValueValidator)
      };
      failures.Add(failure);
    }

    return new ValidationResult(failures);
  }
}
