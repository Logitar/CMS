using Logitar.Cms.Contracts.Configurations;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers.Api;

[ApiController]
[Route("cms/api/configurations")]
public class ConfigurationApiController : ControllerBase
{
  private readonly IConfigurationService _configurationService;

  public ConfigurationApiController(IConfigurationService configurationService)
  {
    _configurationService = configurationService;
  }

  [HttpPost]
  public async Task<ActionResult> InitializeAsync([FromBody] InitializeConfigurationInput input, CancellationToken cancellationToken)
  {
    _ = await _configurationService.InitializeAsync(input, cancellationToken);

    // TODO(fpion): sign-in initial user

    return NoContent();
  }

  [HttpGet("initialized")]
  public async Task<ActionResult<bool>> IsInitializedAsync(CancellationToken cancellationToken)
  {
    bool isInitialized = await _configurationService.GetAsync(cancellationToken) != null;

    return Ok(isInitialized);
  }
}
