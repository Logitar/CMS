using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Sessions.Commands;
using Logitar.Cms.Web.Extensions;
using Logitar.Cms.Web.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  protected new User User => HttpContext.GetUser() ?? throw new InvalidOperationException("An authenticated user is required.");

  public AccountController(IRequestPipeline pipeline)
  {
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

    return Ok(new CurrentUser(session));
  }
}
