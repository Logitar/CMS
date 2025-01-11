using Logitar.Cms.Core.Sessions.Commands;
using Logitar.Cms.Core.Sessions.Models;
using Logitar.Cms.Core.Sessions.Queries;
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

  [HttpGet("{id}")]
  public async Task<ActionResult<SessionModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    SessionModel? session = await _mediator.Send(new ReadSessionQuery(id), cancellationToken);
    return session == null ? NotFound() : Ok(session);
  }

  [HttpPut("renew")]
  public async Task<ActionResult<SessionModel>> RenewAsync([FromBody] RenewSessionPayload payload, CancellationToken cancellationToken)
  {
    SessionModel session = await _mediator.Send(new RenewSessionCommand(payload), cancellationToken);
    return Ok(session);
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<SessionModel>> SignInAsync([FromBody] SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    SessionModel session = await _mediator.Send(new SignInSessionCommand(payload), cancellationToken);
    Uri location = new($"{Request.Scheme}://{Request.Host}/api/sessions/{session.Id}", UriKind.Absolute);
    return Created(location, session);
  }

  [HttpPatch("{id}/sign/out")]
  public async Task<ActionResult<SessionModel>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    SessionModel? session = await _mediator.Send(new SignOutSessionCommand(id), cancellationToken);
    return session == null ? NotFound() : Ok(session);
  }
}
