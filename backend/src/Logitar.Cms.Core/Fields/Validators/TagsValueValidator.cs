using FluentValidation.Results;

namespace Logitar.Cms.Core.Fields.Validators;

internal class TagsValueValidator : IFieldValueValidator
{
  public Task<ValidationResult> ValidateAsync(string inputValue, string propertyName, CancellationToken cancellationToken)
  {
    List<ValidationFailure> failures = new(capacity: 1);

    IReadOnlyCollection<string> values = Parse(inputValue);
    if (values.Count < 1)
    {
      ValidationFailure failure = new(propertyName, "The value cannot be empty.", inputValue)
      {
        ErrorCode = "NotEmptyValidator"
      };
      failures.Add(failure); // TODO(fpion): why?
    }

    return Task.FromResult(new ValidationResult(failures));
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
