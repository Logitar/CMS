using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers.Api;

[ApiController]
[Route("cms/api/configurations")]
public class ConfigurationApiController : ControllerBase
{
  private readonly IConfigurationService _configurationService;
  private readonly ISessionService _sessionService;

  public ConfigurationApiController(IConfigurationService configurationService, ISessionService sessionService)
  {
    _configurationService = configurationService;
    _sessionService = sessionService;
  }

  [HttpPost]
  public async Task<ActionResult<Configuration>> InitializeAsync([FromBody] InitializeConfigurationInput input, CancellationToken cancellationToken)
  {
    Configuration configuration = await _configurationService.InitializeAsync(input, cancellationToken);

    SignInInput signInInput = new()
    {
      Username = input.User.Username,
      Password = input.User.Password,
      IpAddress = HttpContext.GetClientIpAddress(),
      AdditionalInformation = HttpContext.GetAdditionalInformation()
    };
    Session session = await _sessionService.SignInAsync(signInInput, cancellationToken);
    HttpContext.SignIn(session);

    return Ok(configuration);
  }

  [HttpGet("initialized")]
  public async Task<ActionResult<bool>> IsInitializedAsync(CancellationToken cancellationToken)
  {
    bool isInitialized = await _configurationService.GetAsync(cancellationToken) != null;

    return Ok(isInitialized);
  }
}
