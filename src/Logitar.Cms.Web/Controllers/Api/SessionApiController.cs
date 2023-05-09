using Logitar.Cms.Contracts.Sessions;
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
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInInput input, CancellationToken cancellationToken)
  {
    return Ok(await _sessionService.SignInAsync(input, cancellationToken));
  }
}
