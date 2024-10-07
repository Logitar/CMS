using FluentValidation.Results;
using Logitar.Cms.Contracts.Errors;

namespace Logitar.Cms.Core;

public static class FluentValidationExtensions
{
  public static PropertyError ToPropertyError(this ValidationFailure failure)
  {
    return new(failure.ErrorCode, failure.ErrorMessage, failure.PropertyName, failure.AttemptedValue);
  }
}
