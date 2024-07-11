using Logitar.Cms.Contracts.Account;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Sessions.Commands;
using Logitar.Cms.Web.Authentication;
using Logitar.Cms.Web.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
  private readonly IOpenAuthenticationService _openAuthenticationService;
  private readonly IRequestPipeline _pipeline;

  public AccountController(IOpenAuthenticationService openAuthenticationService, IRequestPipeline pipeline)
  {
    _openAuthenticationService = openAuthenticationService;
    _pipeline = pipeline;
  }

  [Authorize] // TODO(fpion): will fail when using API keys
  [HttpGet("profile")]
  public ActionResult<User> GetProfile() // TODO(fpion): other return type
  {
    User user = HttpContext.GetUser() ?? throw new InvalidOperationException("An authenticated user is required.");
    return Ok(user);
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

  [HttpPost("token")]
  public async Task<ActionResult<TokenResponse>> GetTokenAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
  {
    // TODO(fpion): Renew

    SignInSessionPayload signIn = new(payload.Username, payload.Password, isPersistent: true, HttpContext.GetSessionCustomAttributes());
    SignInSessionCommand command = new(signIn);
    Session session = await _pipeline.ExecuteAsync(command, cancellationToken);
    TokenResponse response = await _openAuthenticationService.GetTokenResponseAsync(session, cancellationToken);

    return Ok(response);
  }
}
