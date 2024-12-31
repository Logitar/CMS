using Logitar.Cms.Core;
using Logitar.Cms.Core.Users.Models;
using Logitar.Cms.Web.Extensions;
using Logitar.EventSourcing;

namespace Logitar.Cms.Web;

internal class HttpApplicationContext : IApplicationContext
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public HttpApplicationContext(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public ActorId? ActorId
  {
    get
    {
      HttpContext? context = _httpContextAccessor.HttpContext;
      if (context != null)
      {
        UserModel? user = context.GetUser();
        if (user != null)
        {
          return new ActorId(user.Id);
        }
      }

      return null;
    }
  }
}
