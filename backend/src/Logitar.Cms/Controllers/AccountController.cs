using Logitar.Cms.Authentication;
using Logitar.Cms.Constants;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Sessions.Commands;
using Logitar.Cms.Core.Users.Commands;
using Logitar.Cms.Models.Account;
using Logitar.Cms.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
  private readonly IBearerService _bearerTokenService;
  private readonly IRequestPipeline _pipeline;

  public AccountController(IBearerService bearerTokenService, IRequestPipeline pipeline)
  {
    _bearerTokenService = bearerTokenService;
    _pipeline = pipeline;
  }

  protected new User User => HttpContext.GetUser() ?? throw new InvalidOperationException("The User is required.");

  [Authorize(Policy = Policies.User)]
  [HttpGet("profile")]
  public ActionResult<User> GetProfile() => Ok(User);

  [HttpPost("sign/in")]
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInAccountPayload account, CancellationToken cancellationToken)
  {
    SignInSessionPayload payload = new(account.Username, account.Password)
    {
      IpAddress = HttpContext.GetClientIpAddress(),
      AdditionalInformation = HttpContext.GetAdditionalInformation()
    };
    SignInSessionCommand command = new(payload);
    Session session = await _pipeline.ExecuteAsync(command, cancellationToken);
    HttpContext.SignIn(session);

    return Ok(session);
  }

  [Authorize(Policy = Policies.User)]
  [HttpPost("sign/out/all")]
  public async Task<ActionResult<User>> SignOutAllAsync(CancellationToken cancellationToken)
  {
    SignOutUserCommand command = new(User.Id);
    _ = await _pipeline.ExecuteAsync(command, cancellationToken);
    HttpContext.SignOut();

    return NoContent();
  }

  [Authorize(Policy = Policies.User)]
  [HttpPost("sign/out")]
  public async Task<ActionResult<Session>> SignOutAsync(CancellationToken cancellationToken)
  {
    Guid? sessionId = HttpContext.GetSessionId();
    if (sessionId.HasValue)
    {
      SignOutSessionCommand command = new(sessionId.Value);
      _ = await _pipeline.ExecuteAsync(command, cancellationToken);
    }
    HttpContext.SignOut();

    return NoContent();
  }

  [HttpPost("token")]
  public async Task<ActionResult<TokenResponse>> GetTokenAsync([FromBody] GetTokenPayload getToken, CancellationToken cancellationToken)
  {
    Session session;
    if (string.IsNullOrWhiteSpace(getToken.RefreshToken))
    {
      SignInSessionPayload payload = new(getToken.Username ?? string.Empty, getToken.Password ?? string.Empty)
      {
        IpAddress = HttpContext.GetClientIpAddress(),
        AdditionalInformation = HttpContext.GetAdditionalInformation()
      };
      session = await _pipeline.ExecuteAsync(new SignInSessionCommand(payload), cancellationToken);
    }
    else
    {
      RenewSessionPayload payload = new(getToken.RefreshToken.Trim());
      session = await _pipeline.ExecuteAsync(new RenewSessionCommand(payload), cancellationToken);
    }

    TokenResponse response = await _bearerTokenService.GetTokenResponseAsync(session, cancellationToken);
    return Ok(response);
  }
}
