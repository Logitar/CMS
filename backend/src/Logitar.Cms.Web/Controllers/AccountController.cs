using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Sessions.Commands;
using Logitar.Cms.Web.Models.Account;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public AccountController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<CurrentUser>> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
  {
    Session session = await _pipeline.ExecuteAsync(new SignInSessionCommand(payload.ToPayload(HttpContext)), cancellationToken);

    // TODO(fpion): cookies

    return Ok(new CurrentUser(session));
  }
}
