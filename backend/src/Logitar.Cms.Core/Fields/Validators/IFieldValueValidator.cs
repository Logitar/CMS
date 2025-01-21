using FluentValidation.Results;

namespace Logitar.Cms.Core.Fields.Validators;

public interface IFieldValueValidator
{
  Task<ValidationResult> ValidateAsync(string value, string propertyName, CancellationToken cancellationToken = default);
}
