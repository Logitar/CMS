using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core;
using Logitar.Cms.Core.ContentTypes.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Authorize]
[Route("contents/types/{contentTypeId}/fields")]
public class FieldDefinitionController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public FieldDefinitionController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<CmsContentType>> CreateAsync(Guid contentTypeId, [FromBody] CreateFieldDefinitionPayload payload, CancellationToken cancellationToken)
  {
    CmsContentType contentType = await _pipeline.ExecuteAsync(new CreateFieldDefinitionCommand(contentTypeId, payload), cancellationToken);
    return Ok(contentType);
  }
}
