using Logitar.Cms.Core.Users.Commands;
using Logitar.Cms.Core.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UserController : ControllerBase
{
  private readonly IMediator _mediator;

  public UserController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPatch("authenticate")]
  public async Task<ActionResult<UserModel>> AuthenticateAsync([FromBody] AuthenticateUserPayload payload, CancellationToken cancellationToken)
  {
    UserModel user = await _mediator.Send(new AuthenticateUserCommand(payload), cancellationToken);
    return Ok(user);
  }

  [HttpPatch("{id}/sign/out")]
  public async Task<ActionResult<UserModel>> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    UserModel? user = await _mediator.Send(new SignOutUserCommand(id), cancellationToken);
    return user == null ? NotFound() : Ok(user);
  }
}
