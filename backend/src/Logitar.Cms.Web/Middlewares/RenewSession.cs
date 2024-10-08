using Logitar.Cms.Web.Constants;

namespace Logitar.Cms.Web.Middlewares;

public class RenewSession // TODO(fpion): RenewSession
{
  private readonly RequestDelegate _next;

  public RenewSession(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context/*, IRequestPipeline requestPipeline*/)
  {
    if (!context.GetSessionId().HasValue)
    {
      if (context.Request.Cookies.TryGetValue(Cookies.RefreshToken, out string? refreshToken) && refreshToken != null)
      {
        try
        {
          //RenewSessionPayload payload = new(refreshToken, context.GetSessionCustomAttributes());
          //RenewSessionCommand command = new(payload);
          //Session session = await requestPipeline.ExecuteAsync(command);
          //context.SignIn(session);
        }
        catch (Exception)
        {
          context.Response.Cookies.Delete(Cookies.RefreshToken);
        }
      }
    }

    await _next(context);
  }
}
