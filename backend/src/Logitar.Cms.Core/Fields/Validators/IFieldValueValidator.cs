using FluentValidation.Results;

namespace Logitar.Cms.Core.Fields.Validators;

internal interface IFieldValueValidator
{
  ValidationResult Validate(string value, string propertyName);
}
