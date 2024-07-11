using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Account;
using Logitar.Cms.Contracts.Errors;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Sessions.Commands;
using Logitar.Cms.Web.Authentication;
using Logitar.Cms.Web.Constants;
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

  [Authorize(Policy = Policies.User)]
  [HttpGet("profile")]
  public ActionResult<UserProfile> GetProfile()
  {
    User user = HttpContext.GetUser() ?? throw new InvalidOperationException("An authenticated user is required.");
    UserProfile profile = new(user);
    return Ok(profile);
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
  public async Task<ActionResult<TokenResponse>> GetTokenAsync([FromBody] GetTokenPayload payload, CancellationToken cancellationToken)
  {
    string? refreshToken = string.IsNullOrWhiteSpace(payload.RefreshToken) ? null : payload.RefreshToken.Trim();
    if ((payload.Credentials == null && refreshToken == null) || (payload.Credentials != null && refreshToken != null))
    {
      return BadRequest(new Error("Validation", $"Exactly one of the following must be provided: {nameof(payload.Credentials)}, {nameof(payload.RefreshToken)}."));
    }

    IEnumerable<CustomAttribute> customAttributes = HttpContext.GetSessionCustomAttributes();
    Session session;
    if (payload.RefreshToken != null)
    {
      RenewSessionPayload renewPayload = new(payload.RefreshToken, customAttributes);
      RenewSessionCommand command = new(renewPayload);
      session = await _pipeline.ExecuteAsync(command, cancellationToken);
    }
    else
    {
      SignInPayload credentials = payload.Credentials ?? new SignInPayload();
      SignInSessionPayload signInPayload = new(credentials.Username, credentials.Password, isPersistent: true, customAttributes);
      SignInSessionCommand command = new(signInPayload);
      session = await _pipeline.ExecuteAsync(command, cancellationToken);
    }

    TokenResponse response = await _openAuthenticationService.GetTokenResponseAsync(session, cancellationToken);

    return Ok(response);
  }
}
