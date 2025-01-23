using FluentValidation.Results;

namespace Logitar.Cms.Core.Fields.Validators;

internal class TagsValueValidator : IFieldValueValidator
{
  public Task<ValidationResult> ValidateAsync(string inputValue, string propertyName, CancellationToken cancellationToken)
  {
    List<ValidationFailure> failures = new(capacity: 1);

    if (!TryParse(inputValue, out _))
    {
      ValidationFailure failure = new(propertyName, "The value must be a JSON-serialized string array.", inputValue)
      {
        ErrorCode = nameof(TagsValueValidator)
      };
      failures.Add(failure);
    }

    return Task.FromResult(new ValidationResult(failures));
  }

  private static bool TryParse(string value, out IReadOnlyCollection<string> tags)
  {
    IReadOnlyCollection<string>? values;
    try
    {
      values = JsonSerializer.Deserialize<IReadOnlyCollection<string>>(value);
    }
    catch (Exception)
    {
      values = null;
    }

    if (values == null)
    {
      tags = [];
      return false;
    }

    tags = values;
    return true;
  }
}
