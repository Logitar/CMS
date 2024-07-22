using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Core;
using Logitar.Cms.Core.FieldTypes.Commands;
using Logitar.Cms.Core.FieldTypes.Queries;
using Logitar.Cms.Web.Models.FieldTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/fields/types")]
public class FieldTypeController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public FieldTypeController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<FieldType>> CreateAsync([FromBody] CreateFieldTypePayload payload, CancellationToken cancellationToken)
  {
    FieldType fieldType = await _pipeline.ExecuteAsync(new CreateFieldTypeCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("api/fields/types/{id}", [new KeyValuePair<string, string>("id", fieldType.Id.ToString())]);

    return Created(location, fieldType);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<FieldType>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    FieldType? fieldType = await _pipeline.ExecuteAsync(new ReadFieldTypeQuery(id, UniqueName: null), cancellationToken);
    return fieldType == null ? NotFound() : Ok(fieldType);
  }

  [HttpGet("unique-name:{uniqueName}")]
  public async Task<ActionResult<FieldType>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    FieldType? fieldType = await _pipeline.ExecuteAsync(new ReadFieldTypeQuery(Id: null, uniqueName), cancellationToken);
    return fieldType == null ? NotFound() : Ok(fieldType);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<FieldType>> ReplaceAsync(Guid id, [FromBody] ReplaceFieldTypePayload payload, long? version, CancellationToken cancellationToken)
  {
    FieldType? fieldType = await _pipeline.ExecuteAsync(new ReplaceFieldTypeCommand(id, payload, version), cancellationToken);
    return fieldType == null ? NotFound() : Ok(fieldType);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<FieldType>>> SearchAsync([FromQuery] SearchFieldTypesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<FieldType> fieldTypes = await _pipeline.ExecuteAsync(new SearchFieldTypesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(fieldTypes);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<FieldType>> UpdateAsync(Guid id, [FromBody] UpdateFieldTypePayload payload, CancellationToken cancellationToken)
  {
    FieldType? fieldType = await _pipeline.ExecuteAsync(new UpdateFieldTypeCommand(id, payload), cancellationToken);
    return fieldType == null ? NotFound() : Ok(fieldType);
  }
}
