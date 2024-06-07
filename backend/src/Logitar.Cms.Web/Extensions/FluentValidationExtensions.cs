using FluentValidation.Results;
using Logitar.Cms.Contracts.Errors;

namespace Logitar.Cms.Web.Extensions;

public static class FluentValidationExtensions
{
  public static PropertyError ToPropertyError(this ValidationFailure failure) => new(failure.ErrorCode, failure.ErrorMessage)
  {
    PropertyName = failure.PropertyName,
    AttemptedValue = failure.AttemptedValue is string s ? string.Concat('"', s, '"') : failure.AttemptedValue?.ToString()
  };
}
