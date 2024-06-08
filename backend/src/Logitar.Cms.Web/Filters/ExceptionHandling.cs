using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Contracts.Errors;
using Logitar.Cms.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Cms.Web.Filters;

public class ExceptionHandling : ExceptionFilterAttribute
{
  public override void OnException(ExceptionContext context)
  {
    // TODO(fpion): handle LocaleAlreadyUsedException
    // TODO(fpion): handle Authentication exceptions
    // TODO(fpion): handle Renewal exceptions
    // TODO(fpion): handle SignIn exceptions
    // TODO(fpion): handle SignOut & SignOutEverywhere exceptions

    if (context.Exception is ValidationException validation)
    {
      ValidationError error = new();
      foreach (ValidationFailure failure in validation.Errors)
      {
        error.Add(failure.ToPropertyError());
      }

      context.Result = new BadRequestObjectResult(error);
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }
  }
}
