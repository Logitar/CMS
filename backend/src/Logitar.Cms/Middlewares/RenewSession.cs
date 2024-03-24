using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Sessions.Commands;
using Logitar.Cms.Web.Constants;
using Logitar.Cms.Web.Extensions;

namespace Logitar.Cms.Middlewares;

internal class RenewSession
{
  private readonly RequestDelegate _next;

  public RenewSession(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context, IRequestPipeline requestPipeline)
  {
    if (!context.GetSessionId().HasValue)
    {
      if (context.Request.Cookies.TryGetValue(Cookies.RefreshToken, out string? refreshToken) && refreshToken != null)
      {
        try
        {
          RenewSessionPayload payload = new(refreshToken)
          {
            IpAddress = context.GetClientIpAddress(),
            AdditionalInformation = context.GetAdditionalInformation()
          };
          RenewSessionCommand command = new(payload);
          Session session = await requestPipeline.ExecuteAsync(command);
          context.SignIn(session);
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
