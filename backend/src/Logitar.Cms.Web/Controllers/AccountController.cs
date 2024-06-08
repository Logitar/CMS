using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Sessions.Commands;
using Logitar.Cms.Core.Users.Commands;
using Logitar.Cms.Web.Authentication;
using Logitar.Cms.Web.Extensions;
using Logitar.Cms.Web.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
  private readonly IOpenAuthenticationService _openAuthenticationService;
  private readonly IRequestPipeline _pipeline;

  protected new User User => HttpContext.GetUser() ?? throw new InvalidOperationException("An authenticated user is required.");

  public AccountController(IOpenAuthenticationService openAuthenticationService, IRequestPipeline pipeline)
  {
    _openAuthenticationService = openAuthenticationService;
    _pipeline = pipeline;
  }

  [Authorize]
  [HttpGet("profile")]
  public ActionResult<User> GetProfile() => Ok(User);

  [HttpPost("sign/in")]
  public async Task<ActionResult<CurrentUser>> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
  {
    Session session = await _pipeline.ExecuteAsync(new SignInSessionCommand(payload.ToPayload(HttpContext)), cancellationToken);
    HttpContext.SignIn(session);

    CurrentUser currentUser = new(session);
    return Ok(currentUser);
  }

  [Authorize]
  [HttpPost("sign/out")]
  public async Task<ActionResult> SignOutAsync(CancellationToken cancellationToken)
  {
    Guid? sessionId = HttpContext.GetSessionId();
    if (sessionId.HasValue)
    {
      _ = await _pipeline.ExecuteAsync(new SignOutSessionCommand(sessionId.Value), cancellationToken);
    }
    HttpContext.SignOut();

    return NoContent();
  }

  [Authorize]
  [HttpPost("sign/out/everywhere")]
  public async Task<ActionResult> SignOutEverywhereAsync(CancellationToken cancellationToken)
  {
    _ = await _pipeline.ExecuteAsync(new SignOutUserCommand(User.Id), cancellationToken);
    HttpContext.SignOut();

    return NoContent();
  }

  [HttpPost("token")]
  public async Task<ActionResult<TokenResponse>> GetTokenAsync([FromBody] GetTokenPayload payload, CancellationToken cancellationToken)
  {
    Session session;
    if (payload.RefreshToken != null)
    {
      session = await _pipeline.ExecuteAsync(new RenewSessionCommand(payload.ToRenewPayload(HttpContext)), cancellationToken);
    }
    else if (payload.Credentials != null)
    {
      session = await _pipeline.ExecuteAsync(new SignInSessionCommand(payload.ToSignInPayload(HttpContext)), cancellationToken);
    }
    else
    {
      return BadRequest(); // TODO(fpion): error detail
    }

    TokenResponse response = await _openAuthenticationService.GetTokenResponseAsync(session, cancellationToken);
    return Ok(response);
  }
}
