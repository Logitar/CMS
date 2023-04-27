using Logitar.Cms.Core;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Sessions;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Cms.Web.Filters;

internal class ExceptionHandlingFilter : IExceptionFilter
{
  private static readonly IReadOnlyDictionary<Type, Action<ExceptionContext>> _handlers = new Dictionary<Type, Action<ExceptionContext>>
  {
    [typeof(ConfigurationAlreadyInitializedException)] = HandleConfigurationAlreadyInitializedException,
    [typeof(InvalidCredentialsException)] = HandleInvalidCredentialsException,
    [typeof(InvalidLocaleException)] = HandleInvalidLocaleException,
    [typeof(SessionIsNotActiveException)] = HandleSessionIsNotActiveException
  };

  public void OnException(ExceptionContext context)
  {
    if (_handlers.TryGetValue(context.Exception.GetType(), out Action<ExceptionContext>? handler))
    {
      handler(context);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is AggregateNotFoundException)
    {
      context.Result = new NotFoundResult();
      context.ExceptionHandled = true;
    }
  }

  private static void HandleConfigurationAlreadyInitializedException(ExceptionContext context)
  {
    context.Result = new JsonResult(new { Code = GetCode(context.Exception) })
    {
      StatusCode = StatusCodes.Status403Forbidden
    };
  }

  private static void HandleInvalidCredentialsException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(new { Code = GetCode(context.Exception) });
  }

  private static void HandleInvalidLocaleException(ExceptionContext context)
  {
    if (context.Exception is IPropertyFailure propertyFailure)
    {
      context.Result = new BadRequestObjectResult(HandlePropertyFailure(propertyFailure));
    }
  }

  private static void HandleSessionIsNotActiveException(ExceptionContext context)
  {
    context.Result = new BadRequestObjectResult(new { Code = GetCode(context.Exception) });
  }

  private static string GetCode(object value) => value.GetType().Name.Remove(nameof(Exception));

  private static object HandlePropertyFailure(IPropertyFailure propertyFailure) => new
  {
    Code = GetCode(propertyFailure),
    propertyFailure.PropertyName,
    propertyFailure.AttemptedValue
  };
}
