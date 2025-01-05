using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Fields.Commands;
using Logitar.Cms.Core.Fields.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/contents/types/{contentTypeId}/fields")]
public class FieldDefinitionController : ControllerBase
{
  private readonly IMediator _mediator;

  public FieldDefinitionController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<ContentTypeModel>> CreateFieldAsync(Guid contentTypeId, [FromBody] CreateOrReplaceFieldDefinitionPayload payload, CancellationToken cancellationToken)
  {
    ContentTypeModel? contentType = await _mediator.Send(new CreateOrReplaceFieldDefinitionCommand(contentTypeId, FieldId: null, payload), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }

  [HttpPut("{fieldId}")]
  public async Task<ActionResult<ContentTypeModel>> ReplaceFieldAsync(Guid contentTypeId, Guid fieldId, [FromBody] CreateOrReplaceFieldDefinitionPayload payload, CancellationToken cancellationToken)
  {
    ContentTypeModel? contentType = await _mediator.Send(new CreateOrReplaceFieldDefinitionCommand(contentTypeId, fieldId, payload), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }

  [HttpPatch("{fieldId}")]
  public async Task<ActionResult<ContentTypeModel>> UpdateFieldAsync(Guid contentTypeId, Guid fieldId, [FromBody] UpdateFieldDefinitionPayload payload, CancellationToken cancellationToken)
  {
    ContentTypeModel? contentType = await _mediator.Send(new UpdateFieldDefinitionCommand(contentTypeId, fieldId, payload), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }
}
