using Logitar.Cms.Core.Fields.Commands;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Fields.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/fields/types")]
public class FieldTypeController : ControllerBase
{
  private readonly IMediator _mediator;

  public FieldTypeController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<FieldTypeModel>> CreateAsync([FromBody] CreateOrReplaceFieldTypePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldTypeResult result = await _mediator.Send(new CreateOrReplaceFieldTypeCommand(Id: null, payload, Version: null), cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<FieldTypeModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    FieldTypeModel? fieldType = await _mediator.Send(new ReadFieldTypeQuery(id, UniqueName: null), cancellationToken);
    return fieldType == null ? NotFound() : Ok(fieldType);
  }

  [HttpGet("unique-name:{uniqueName}")]
  public async Task<ActionResult<FieldTypeModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    FieldTypeModel? fieldType = await _mediator.Send(new ReadFieldTypeQuery(Id: null, uniqueName), cancellationToken);
    return fieldType == null ? NotFound() : Ok(fieldType);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<FieldTypeModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceFieldTypePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldTypeResult result = await _mediator.Send(new CreateOrReplaceFieldTypeCommand(id, payload, version), cancellationToken);
    return ToActionResult(result);
  }

  //[HttpGet]
  //public async Task<ActionResult<SearchResults<FieldTypeModel>>> SearchAsync([FromQuery] SearchFieldTypesParameters parameters, CancellationToken cancellationToken)
  //{
  //  SearchResults<FieldTypeModel> fieldTypes = await _mediator.Send(new SearchFieldTypesQuery(parameters.ToPayload()), cancellationToken);
  //  return Ok(fieldTypes);
  //} // TODO(fpion): implement

  [HttpPatch("{id}")]
  public async Task<ActionResult<FieldTypeModel>> UpdateAsync(Guid id, [FromBody] UpdateFieldTypePayload payload, CancellationToken cancellationToken)
  {
    FieldTypeModel? fieldType = await _mediator.Send(new UpdateFieldTypeCommand(id, payload), cancellationToken);
    return fieldType == null ? NotFound() : Ok(fieldType);
  }

  private ActionResult<FieldTypeModel> ToActionResult(CreateOrReplaceFieldTypeResult result)
  {
    if (result.FieldType == null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/fields/types/{result.FieldType.Id}", UriKind.Absolute);
      return Created(location, result.FieldType);
    }

    return Ok(result.FieldType);
  }
}
