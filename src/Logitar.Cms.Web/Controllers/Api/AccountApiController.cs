using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Web.Constants;
using Logitar.Cms.Web.Extensions;
using Logitar.Cms.Web.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers.Api;

[ApiController]
[Route("cms/api/account")]
public class AccountApiController : ControllerBase
{
  private readonly ISessionService _sessionService;
  private readonly IUserService _userService;

  public AccountApiController(ISessionService sessionService, IUserService userService)
  {
    _sessionService = sessionService;
    _userService = userService;
  }

  [Authorize(Policy = Policies.User)]
  [HttpGet("profile")]
  public ActionResult<User> GetProfile()
  {
    return Ok(HttpContext.GetUser());
  }

  [Authorize(Policy = Policies.User)]
  [HttpPut("profile")]
  public async Task<ActionResult<User>> SaveProfileAsync([FromBody] SaveProfileInput input, CancellationToken cancellationToken)
  {
    UpdateUserInput updateUserInput = new()
    {
      Email = input.Email,
      FirstName = input.FirstName,
      LastName = input.LastName,
      Locale = input.Locale,
      Picture = input.Picture
    };

    return Ok(await _userService.UpdateAsync(HttpContext.GetUser()!.Id, updateUserInput, cancellationToken));
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult> SignInAsync([FromBody] AccountSignInInput input, CancellationToken cancellationToken)
  {
    SignInInput signInInput = new()
    {
      Username = input.Username,
      Password = input.Password,
      Remember = input.Remember,
      IpAddress = HttpContext.GetClientIpAddress(),
      AdditionalInformation = HttpContext.GetAdditionalInformation()
    };
    Session session = await _sessionService.SignInAsync(signInInput, cancellationToken);
    HttpContext.SignIn(session);

    return NoContent();
  }

  [HttpPost("sign/out")]
  public async Task<ActionResult> SignOutAsync(CancellationToken cancellationToken)
  {
    Guid? sessionId = HttpContext.GetSessionId();
    if (sessionId.HasValue)
    {
      _ = await _sessionService.SignOutAsync(sessionId.Value, cancellationToken);
      HttpContext.SignOut();
    }

    return NoContent();
  }

  [Authorize(Policy = Policies.User)]
  [HttpPost("sign/out/all")]
  public async Task<ActionResult> SignOutUserAsync(CancellationToken cancellationToken)
  {
    _ = await _sessionService.SignOutUserAsync(HttpContext.GetUser()!.Id, cancellationToken);
    HttpContext.SignOut();

    return NoContent();
  }
}
