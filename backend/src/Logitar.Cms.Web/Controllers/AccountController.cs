using Logitar.Cms.Contracts.Account;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Sessions.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Route("api/account")]
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
    SignInSessionPayload signIn = new(payload.Username, payload.Password, isPersistent: true, HttpContext.GetSessionCustomAttributes());
    SignInSessionCommand command = new(signIn);
    Session session = await _pipeline.ExecuteAsync(command, cancellationToken);
    HttpContext.SignIn(session);

    return Ok(new CurrentUser(session));
  }
}
