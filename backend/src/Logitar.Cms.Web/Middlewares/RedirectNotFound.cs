﻿using Microsoft.AspNetCore.Http.Extensions;

namespace Logitar.Cms.Web.Middlewares;

public class RedirectNotFound
{
  private readonly RequestDelegate _next;

  public RedirectNotFound(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    await _next(context);

    if (context.Response.StatusCode == StatusCodes.Status404NotFound && !context.Request.Path.StartsWithSegments("/api"))
    {
      context.Response.Redirect($"/cms/{UriHelper.GetEncodedPathAndQuery(context.Request).Trim('/')}");
    }
  }
}
