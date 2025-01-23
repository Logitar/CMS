using Logitar.Cms.Core;
using Logitar.Cms.Core.Sessions.Commands;
using Logitar.Cms.Core.Sessions.Models;
using Logitar.Cms.Core.Users.Commands;
using Logitar.Cms.Core.Users.Models;
using Logitar.Cms.Web.Authentication;
using Logitar.Cms.Web.Extensions;
using Logitar.Cms.Web.Models.Account;
using Logitar.Identity.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Route("api")]
public class AccountController : ControllerBase
{
  private readonly IMediator _mediator;
  private readonly IOpenAuthenticationService _openAuthenticationService;

  public AccountController(IMediator mediator, IOpenAuthenticationService openAuthenticationService)
  {
    _mediator = mediator;
    _openAuthenticationService = openAuthenticationService;
  }

  [HttpGet("profile")]
  [Authorize]
  public ActionResult<UserProfile> GetProfile()
  {
    UserModel user = HttpContext.GetUser() ?? throw new InvalidOperationException("An authorized user is required.");
    UserProfile profile = new(user);
    return Ok(profile);
  }

  [HttpPost("auth/token")]
  public async Task<ActionResult<TokenResponse>> GetTokenAsync([FromBody] GetTokenPayload tokenPayload, CancellationToken cancellationToken)
  {
    try
    {
      SessionModel session;
      if (string.IsNullOrWhiteSpace(tokenPayload.RefreshToken))
      {
        SignInSessionPayload payload = new(tokenPayload.Username, tokenPayload.Password, id: null, isPersistent: true, HttpContext.GetSessionCustomAttributes());
        session = await _mediator.Send(new SignInSessionCommand(payload), cancellationToken);
      }
      else
      {
        RenewSessionPayload payload = new(tokenPayload.RefreshToken.Trim(), HttpContext.GetSessionCustomAttributes());
        session = await _mediator.Send(new RenewSessionCommand(payload), cancellationToken);
      }

      TokenResponse response = await _openAuthenticationService.GetTokenResponseAsync(session, cancellationToken);
      return Ok(response);
    }
    catch (InvalidCredentialsException)
    {
      return InvalidCredentials();
    }
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<CurrentUser>> SignInAsync([FromBody] SignInPayload credentials, CancellationToken cancellationToken)
  {
    try
    {
      SignInSessionPayload payload = new(credentials.Username, credentials.Password, id: null, isPersistent: true, HttpContext.GetSessionCustomAttributes());
      SessionModel session = await _mediator.Send(new SignInSessionCommand(payload), cancellationToken);
      HttpContext.SignIn(session);
      return Ok(new CurrentUser(session));
    }
    catch (InvalidCredentialsException)
    {
      return InvalidCredentials();
    }
  }

  [HttpPost("sign/out")]
  [Authorize]
  public async Task<ActionResult> SignOutAsync(bool everywhere, CancellationToken cancellationToken)
  {
    if (everywhere)
    {
      UserModel? user = HttpContext.GetUser();
      if (user != null)
      {
        await _mediator.Send(new SignOutUserCommand(user.Id), cancellationToken);
      }
    }
    else
    {
      SessionModel? session = HttpContext.GetSession();
      if (session != null)
      {
        await _mediator.Send(new SignOutSessionCommand(session.Id), cancellationToken);
      }
    }

    return NoContent();
  }

  private ObjectResult InvalidCredentials()
  {
    Error error = new("InvalidCredentials", "The specified credentials did not match.");
    return Problem(
      detail: error.Message,
      instance: HttpContext.Request.GetDisplayUrl(),
      statusCode: StatusCodes.Status400BadRequest,
      title: "Invalid Credentials",
      type: null,
      extensions: new Dictionary<string, object?> { ["error"] = error });
  }
}
