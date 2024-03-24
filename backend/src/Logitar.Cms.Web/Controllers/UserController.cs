using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Users.Commands;
using Logitar.Cms.Core.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UserController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public UserController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPatch("authenticate")]
  public async Task<ActionResult<User>> AuthenticateAsync([FromBody] AuthenticateUserPayload payload, CancellationToken cancellationToken)
  {
    User user = await _pipeline.ExecuteAsync(new AuthenticateUserCommand(payload), cancellationToken);
    return Ok(user);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<User>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    User? user = await _pipeline.ExecuteAsync(new ReadUserQuery(id, Username: null), cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpGet("username:{username}")]
  public async Task<ActionResult<User>> ReadAsync(string username, CancellationToken cancellationToken)
  {
    User? user = await _pipeline.ExecuteAsync(new ReadUserQuery(Id: null, username), cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }

  [HttpPut("{id}/sign/out")]
  public async Task<ActionResult<User>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    User? user = await _pipeline.ExecuteAsync(new SignOutUserCommand(id), cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }
}
