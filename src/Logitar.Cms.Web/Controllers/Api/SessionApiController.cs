using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers.Api;

[ApiController]
[Route("cms/api/sessions")]
public class SessionApiController : ControllerBase
{
  private readonly ISessionService _sessionService;

  public SessionApiController(ISessionService sessionService)
  {
    _sessionService = sessionService;
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult> SignInAsync([FromBody] SignInInput input, CancellationToken cancellationToken)
  {
    input.IpAddress = HttpContext.GetClientIpAddress();
    input.AdditionalInformation = HttpContext.GetAdditionalInformation();

    Session session = await _sessionService.SignInAsync(input, cancellationToken);
    HttpContext.SignIn(session);

    return NoContent();
  }
}
