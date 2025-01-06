using Logitar.Cms.Core.Sessions.Commands;
using Logitar.Cms.Core.Sessions.Models;
using Logitar.Cms.Web.Authentication;
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
  private readonly IOpenAuthenticationService _openAuthenticationService;

  public AccountController(IMediator mediator, IOpenAuthenticationService openAuthenticationService)
  {
    _mediator = mediator;
    _openAuthenticationService = openAuthenticationService;
  }

  [HttpPost("auth/token")]
  public async Task<ActionResult<TokenResponse>> GetTokenAsync([FromBody] GetTokenPayload tokenPayload, CancellationToken cancellationToken)
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

  [HttpPost("auth/sign/in")]
  public async Task<ActionResult<CurrentUser>> SignInAsync([FromBody] SignInPayload credentials, CancellationToken cancellationToken)
  {
    SignInSessionPayload payload = new(credentials.Username, credentials.Password, id: null, isPersistent: true, HttpContext.GetSessionCustomAttributes());
    SessionModel session = await _mediator.Send(new SignInSessionCommand(payload), cancellationToken);
    HttpContext.SignIn(session);
    return Ok(new CurrentUser(session));
  }
}
