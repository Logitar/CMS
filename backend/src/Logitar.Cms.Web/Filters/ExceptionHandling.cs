using FluentValidation;
using Logitar.Cms.Contracts.Errors;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Localization;
using Logitar.Cms.Core.Shared;
using Logitar.Identity.Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Cms.Web.Filters;

public class ExceptionHandling : ExceptionFilterAttribute
{
  private static readonly Dictionary<Type, Func<ExceptionContext, ActionResult>> _handlers = new()
  {
    [typeof(ValidationException)] = HandleValidationException
  };

  public override void OnException(ExceptionContext context)
  {
    if (_handlers.TryGetValue(context.Exception.GetType(), out Func<ExceptionContext, ActionResult>? handler))
    {
      context.Result = handler(context);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is AggregateNotFoundException aggregateNotFound)
    {
      context.Result = new NotFoundObjectResult(new ValidationFailure(aggregateNotFound.GetErrorCode(), AggregateNotFoundException.ErrorMessage)
      {
        PropertyName = aggregateNotFound.PropertyName,
        AttemptedValue = aggregateNotFound.Id
      });
      context.ExceptionHandled = true;
    }
    else if (context.Exception is InvalidCredentialsException)
    {
      context.Result = new BadRequestObjectResult(new Error("InvalidCredentials", InvalidCredentialsException.ErrorMessage));
      context.ExceptionHandled = true;
    }
    else if (context.Exception is LocaleAlreadyUsedException localeAlreadyUsed)
    {
      context.Result = new ConflictObjectResult(new ValidationFailure(localeAlreadyUsed.GetErrorCode(), LocaleAlreadyUsedException.ErrorMessage)
      {
        PropertyName = localeAlreadyUsed.PropertyName,
        AttemptedValue = localeAlreadyUsed.Locale
      });
      context.ExceptionHandled = true;
    }
    else if (context.Exception is TooManyResultsException)
    {
      context.Result = new BadRequestObjectResult(new Error(context.Exception.GetErrorCode(), TooManyResultsException.ErrorMessage));
      context.ExceptionHandled = true;
    }
    else if (context.Exception is Core.Shared.UniqueNameAlreadyUsedException uniqueNameAlreadyUsed)
    {
      context.Result = new ConflictObjectResult(new ValidationFailure(uniqueNameAlreadyUsed.GetErrorCode(), Core.Shared.UniqueNameAlreadyUsedException.ErrorMessage)
      {
        PropertyName = uniqueNameAlreadyUsed.PropertyName,
        AttemptedValue = uniqueNameAlreadyUsed.UniqueName
      });
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }
  }

  private static BadRequestObjectResult HandleValidationException(ExceptionContext context)
  {
    IReadOnlyCollection<ValidationFailure> failures = ((ValidationException)context.Exception).Errors.Select(e => new ValidationFailure(e.ErrorCode, e.ErrorMessage)
    {
      PropertyName = e.PropertyName?.CleanTrim(),
      AttemptedValue = e.AttemptedValue
    }).ToArray();
    return new BadRequestObjectResult(new ValidationError(context.Exception.GetErrorCode(), "Validation failed.", failures));
  }
}
