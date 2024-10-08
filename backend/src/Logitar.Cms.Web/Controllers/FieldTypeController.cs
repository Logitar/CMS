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
  public async Task<ActionResult<FieldTypeModel>> CreateAsync([FromBody] CreateOrReplaceFieldTypePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldTypeResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceFieldTypeCommand(Id: null, payload, Version: null), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<FieldTypeModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    FieldTypeModel? fieldType = await _pipeline.ExecuteAsync(new ReadFieldTypeQuery(id, UniqueName: null), cancellationToken);
    return GetActionResult(fieldType);
  }

  [HttpGet("unique-name:{uniqueName}")]
  public async Task<ActionResult<FieldTypeModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    FieldTypeModel? fieldType = await _pipeline.ExecuteAsync(new ReadFieldTypeQuery(Id: null, uniqueName), cancellationToken);
    return GetActionResult(fieldType);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<FieldTypeModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceFieldTypePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldTypeResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceFieldTypeCommand(id, payload, version), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<FieldTypeModel>>> SearchAsync([FromQuery] SearchFieldTypesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<FieldTypeModel> fieldTypes = await _pipeline.ExecuteAsync(new SearchFieldTypesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(fieldTypes);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<FieldTypeModel>> UpdateAsync(Guid id, [FromBody] UpdateFieldTypePayload payload, CancellationToken cancellationToken)
  {
    FieldTypeModel? fieldType = await _pipeline.ExecuteAsync(new UpdateFieldTypeCommand(id, payload), cancellationToken);
    return GetActionResult(fieldType);
  }

  private ActionResult<FieldTypeModel> GetActionResult(CreateOrReplaceFieldTypeResult result)
  {
    return GetActionResult(result.FieldType, result.Created);
  }
  private ActionResult<FieldTypeModel> GetActionResult(FieldTypeModel? fieldType, bool created = false)
  {
    if (fieldType == null)
    {
      return NotFound();
    }
    else if (created)
    {
      Uri location = HttpContext.BuildLocation("api/fields/types/{id}", [new KeyValuePair<string, string>("id", fieldType.Id.ToString())]);
      return Created(location, fieldType);
    }

    return Ok(fieldType);
  }
}
