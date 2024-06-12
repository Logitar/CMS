using FluentValidation.Results;
using Logitar.Cms.Contracts.Errors;

namespace Logitar.Cms.Web.Extensions;

public static class FluentValidationExtensions
{
  public static PropertyError ToPropertyError(this ValidationFailure failure)
  {
    return new PropertyError(failure.ErrorCode, failure.ErrorMessage, failure.PropertyName, failure.AttemptedValue);
  }
}
