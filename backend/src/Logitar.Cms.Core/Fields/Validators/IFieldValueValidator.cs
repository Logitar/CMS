using FluentValidation.Results;

namespace Logitar.Cms.Core.Fields.Validators;

internal interface IFieldValueValidator
{
  Task<ValidationResult> ValidateAsync(string value, string propertyName, CancellationToken cancellationToken = default);
}
