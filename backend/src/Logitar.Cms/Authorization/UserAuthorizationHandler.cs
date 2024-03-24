using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Web.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Logitar.Cms.Authorization;

internal class UserAuthorizationHandler : AuthorizationHandler<UserAuthorizationRequirement>
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public UserAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAuthorizationRequirement requirement)
  {
    User? user = _httpContextAccessor.HttpContext?.GetUser();
    if (user == null)
    {
      context.Fail(new AuthorizationFailureReason(this, "An authenticated user is required."));
    }
    else
    {
      context.Succeed(requirement);
    }
    return Task.CompletedTask;
  }
}
