using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Web.Constants;
using Logitar.Cms.Web.Extensions;
using Logitar.Cms.Web.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers.Api;

[ApiController]
[Route("cms/api/account")]
public class AccountApiController : ControllerBase
{
  private readonly ISessionService _sessionService;

  public AccountApiController(ISessionService sessionService)
  {
    _sessionService = sessionService;
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult> SignInAsync([FromBody] AccountSignInInput input, CancellationToken cancellationToken)
  {
    SignInInput signInInput = new()
    {
      Username = input.Username,
      Password = input.Password,
      Remember = input.Remember,
      IpAddress = HttpContext.GetClientIpAddress(),
      AdditionalInformation = HttpContext.GetAdditionalInformation()
    };
    Session session = await _sessionService.SignInAsync(signInInput, cancellationToken);
    HttpContext.SignIn(session);

    return NoContent();
  }

  [HttpPost("sign/out")]
  public async Task<ActionResult> SignOutAsync(CancellationToken cancellationToken)
  {
    Guid? sessionId = HttpContext.GetSessionId();
    if (sessionId.HasValue)
    {
      _ = await _sessionService.SignOutAsync(sessionId.Value, cancellationToken);
      HttpContext.SignOut();
    }

    return NoContent();
  }

  [Authorize(Policy = Policies.User)]
  [HttpPost("sign/out/all")]
  public async Task<ActionResult> SignOutUserAsync(CancellationToken cancellationToken)
  {
    _ = await _sessionService.SignOutUserAsync(HttpContext.GetUser()!.Id, cancellationToken);
    HttpContext.SignOut();

    return NoContent();
  }
}
