using FluentValidation.Results;
using System.Text.Json;

namespace Logitar.Cms.Core.Fields.Validators;

internal class TagsValueValidator : IFieldValueValidator
{
  public ValidationResult Validate(string inputValue, string propertyName)
  {
    List<ValidationFailure> failures = new(capacity: 1);

    IReadOnlyCollection<string> values = Parse(inputValue);
    if (values.Count < 1)
    {
      ValidationFailure failure = new(propertyName, "The value cannot be empty.", inputValue)
      {
        ErrorCode = "NotEmptyValidator"
      };
      failures.Add(failure);
    }

    return new ValidationResult(failures);
  }

  private static IReadOnlyCollection<string> Parse(string value)
  {
    IReadOnlyCollection<string>? values = null;
    try
    {
      values = JsonSerializer.Deserialize<IReadOnlyCollection<string>>(value);
    }
    catch (Exception)
    {
    }

    return values ?? [];
  }
}
