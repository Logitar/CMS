using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Contracts.Errors;
using Logitar.Cms.Core;
using Logitar.Identity.Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Cms.Web.Filters;

public class ExceptionHandling : ExceptionFilterAttribute
{
  public override void OnException(ExceptionContext context)
  {
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
    else if (context.Exception is BadRequestException badRequest)
    {
      context.Result = new BadRequestObjectResult(badRequest.Error);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is NotFoundException notFound)
    {
      context.Result = new NotFoundObjectResult(notFound.Error);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is ConflictException conflict)
    {
      context.Result = new ConflictObjectResult(conflict.Error);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is InvalidCredentialsException)
    {
      context.Result = new BadRequestObjectResult(new InvalidCredentialsError());
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }
  }
}
