using FluentValidation.Results;
using Logitar.Cms.Core.Fields.Settings;
using System.Text.Json;

namespace Logitar.Cms.Core.Fields.Validators;

internal class SelectValueValidator : IFieldValueValidator
{
  private readonly ISelectSettings _settings;

  public SelectValueValidator(ISelectSettings settings)
  {
    _settings = settings;
  }

  public ValidationResult Validate(string inputValue, string propertyName)
  {
    IReadOnlyCollection<string> values = Parse(inputValue);
    List<ValidationFailure> failures = new(capacity: 1 + values.Count);

    if (values.Count < 0)
    {
      ValidationFailure failure = new(propertyName, "The value cannot be empty.", inputValue)
      {
        ErrorCode = "NotEmptyValidator"
      };
      failures.Add(failure);
    }
    else if (values.Count > 1 && !_settings.IsMultiple)
    {
      ValidationFailure failure = new(propertyName, "Exactly one value is allowed.", inputValue)
      {
        ErrorCode = "MultipleValidator"
      };
      failures.Add(failure);
    }

    HashSet<string> allowedValues = _settings.Options.Select(option => option.Value ?? option.Text).ToHashSet();
    foreach (string value in values)
    {
      if (!allowedValues.Contains(value))
      {
        ValidationFailure failure = new(propertyName, "The value is not included within select options.", value)
        {
          ErrorCode = "OptionValidator"
        };
      }
    }

    return new ValidationResult(failures);
  }

  private static IReadOnlyCollection<string> Parse(string value)
  {
    IReadOnlyCollection<string>? values = null;
    if (value.StartsWith('[') && value.EndsWith(']'))
    {
      try
      {
        values = JsonSerializer.Deserialize<IReadOnlyCollection<string>>(value);
      }
      catch (Exception)
      {
      }
    }

    return values ?? [value];
  }
}
