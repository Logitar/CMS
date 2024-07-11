using Logitar.Cms.Contracts.Users;
using Microsoft.AspNetCore.Authorization;

namespace Logitar.Cms.Web.Authorization;

public class UserAuthorizationHandler : AuthorizationHandler<UserAuthorizationRequirement>
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public UserAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAuthorizationRequirement requirement)
  {
    HttpContext? httpContext = _httpContextAccessor.HttpContext;
    if (httpContext != null)
    {
      User? user = httpContext.GetUser();
      if (user == null)
      {
        context.Fail(new AuthorizationFailureReason(this, "The actor must be an authenticated user."));
      }
      else
      {
        context.Succeed(requirement);
      }
    }

    return Task.CompletedTask;
  }
}
