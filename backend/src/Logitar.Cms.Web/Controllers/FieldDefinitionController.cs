using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core;
using Logitar.Cms.Core.ContentTypes.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/contents/types/{contentTypeId}/fields/definitions")]
public class FieldDefinitionController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public FieldDefinitionController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<ContentsType>> CreateAsync(Guid contentTypeId, [FromBody] CreateFieldDefinitionPayload payload, CancellationToken cancellationToken)
  {
    ContentsType? contentType = await _pipeline.ExecuteAsync(new CreateFieldDefinitionCommand(contentTypeId, payload), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }

  [HttpPut("{fieldId}")]
  public async Task<ActionResult<ContentsType>> ReplaceAsync(Guid contentTypeId, Guid fieldId, [FromBody] ReplaceFieldDefinitionPayload payload, long? version, CancellationToken cancellationToken)
  {
    ContentsType? contentType = await _pipeline.ExecuteAsync(new ReplaceFieldDefinitionCommand(contentTypeId, fieldId, payload, version), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }

  [HttpPatch("{fieldId}")]
  public async Task<ActionResult<ContentsType>> UpdateAsync(Guid contentTypeId, Guid fieldId, [FromBody] UpdateFieldDefinitionPayload payload, CancellationToken cancellationToken)
  {
    ContentsType? contentType = await _pipeline.ExecuteAsync(new UpdateFieldDefinitionCommand(contentTypeId, fieldId, payload), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }
}
