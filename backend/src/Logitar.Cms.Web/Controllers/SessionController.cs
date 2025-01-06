using Logitar.Cms.Core.Sessions.Commands;
using Logitar.Cms.Core.Sessions.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/sessions")]
public class SessionController : ControllerBase
{
  private readonly IMediator _mediator;

  public SessionController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<SessionModel>> SignInAsync([FromBody] SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    SessionModel session = await _mediator.Send(new SignInSessionCommand(payload), cancellationToken);
    Uri location = new($"{Request.Scheme}://{Request.Host}/api/sessions/{session.Id}", UriKind.Absolute);
    return Created(location, session);
  }
}
