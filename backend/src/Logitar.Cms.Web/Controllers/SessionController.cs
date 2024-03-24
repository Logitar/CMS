using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Sessions.Commands;
using Logitar.Cms.Core.Sessions.Queries;
using Logitar.Cms.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/sessions")]
public class SessionController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public SessionController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Session>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Session? session = await _pipeline.ExecuteAsync(new ReadSessionQuery(id), cancellationToken);
    return session == null ? NotFound() : Ok(session);
  }

  [HttpPut("renew")]
  public async Task<ActionResult<Session>> RenewAsync([FromBody] RenewSessionPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _pipeline.ExecuteAsync(new RenewSessionCommand(payload), cancellationToken));
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    Session session = await _pipeline.ExecuteAsync(new SignInSessionCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("api/sessions/{id}", new Dictionary<string, string>
    {
      ["id"] = session.Id.ToString()
    });
    return Created(location, session);
  }

  [HttpPatch("{id}/sign/out")]
  public async Task<ActionResult<Session>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    Session? session = await _pipeline.ExecuteAsync(new SignOutSessionCommand(id), cancellationToken);
    return session == null ? NotFound() : Ok(session);
  }
}
