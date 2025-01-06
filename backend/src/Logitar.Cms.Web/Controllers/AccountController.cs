using Logitar.Cms.Core.Sessions.Commands;
using Logitar.Cms.Core.Sessions.Models;
using Logitar.Cms.Web.Extensions;
using Logitar.Cms.Web.Models.Account;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Route("api")]
public class AccountController : ControllerBase
{
  private readonly IMediator _mediator;

  public AccountController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost("auth/sign/in")]
  public async Task<ActionResult<CurrentUser>> SignInAsync([FromBody] SignInPayload credentials, CancellationToken cancellationToken)
  {
    SignInSessionPayload payload = new(credentials.Username, credentials.Password, id: null, isPersistent: true, HttpContext.GetSessionCustomAttributes());
    SessionModel session = await _mediator.Send(new SignInSessionCommand(payload), cancellationToken);
    HttpContext.SignIn(session);
    return Ok(new CurrentUser(session));
  }
}
