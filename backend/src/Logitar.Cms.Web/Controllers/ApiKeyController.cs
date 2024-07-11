using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Cms.Core;
using Logitar.Cms.Core.ApiKeys.Commands;
using Logitar.Cms.Core.ApiKeys.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/keys")]
public class ApiKeyController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public ApiKeyController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<ApiKey>> CreateAsync([FromBody] CreateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    ApiKey apiKey = await _pipeline.ExecuteAsync(new CreateApiKeyCommand(payload), cancellationToken);
    Uri uri = HttpContext.BuildLocation("api/keys/{id}", [new KeyValuePair<string, string>("id", apiKey.Id.ToString())]);

    return Created(uri, apiKey);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ApiKey>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ApiKey? apiKey = await _pipeline.ExecuteAsync(new ReadApiKeyQuery(id), cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }
}
