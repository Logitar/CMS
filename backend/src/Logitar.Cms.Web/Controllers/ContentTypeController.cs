using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Core;
using Logitar.Cms.Core.ContentTypes.Commands;
using Logitar.Cms.Core.ContentTypes.Queries;
using Logitar.Cms.Web.Models.ContentTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/contents/types")]
public class ContentTypeController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public ContentTypeController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<ContentTypeModel>> CreateAsync([FromBody] CreateOrReplaceContentTypePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceContentTypeResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceContentTypeCommand(Id: null, payload, Version: null), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ContentTypeModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ContentTypeModel? contentType = await _pipeline.ExecuteAsync(new ReadContentTypeQuery(id, UniqueName: null), cancellationToken);
    return GetActionResult(contentType);
  }

  [HttpGet("unique-name:{uniqueName}")]
  public async Task<ActionResult<ContentTypeModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    ContentTypeModel? contentType = await _pipeline.ExecuteAsync(new ReadContentTypeQuery(Id: null, uniqueName), cancellationToken);
    return GetActionResult(contentType);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ContentTypeModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceContentTypePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceContentTypeResult result = await _pipeline.ExecuteAsync(new CreateOrReplaceContentTypeCommand(id, payload, version), cancellationToken);
    return GetActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<ContentTypeModel>>> SearchAsync([FromQuery] SearchContentTypesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<ContentTypeModel> contentTypes = await _pipeline.ExecuteAsync(new SearchContentTypesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(contentTypes);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<ContentTypeModel>> UpdateAsync(Guid id, [FromBody] UpdateContentTypePayload payload, CancellationToken cancellationToken)
  {
    ContentTypeModel? contentType = await _pipeline.ExecuteAsync(new UpdateContentTypeCommand(id, payload), cancellationToken);
    return GetActionResult(contentType);
  }

  private ActionResult<ContentTypeModel> GetActionResult(CreateOrReplaceContentTypeResult result)
  {
    return GetActionResult(result.ContentType, result.Created);
  }
  private ActionResult<ContentTypeModel> GetActionResult(ContentTypeModel? contentType, bool created = false)
  {
    if (contentType == null)
    {
      return NotFound();
    }
    else if (created)
    {
      Uri location = HttpContext.BuildLocation("api/contents/types/{id}", [new KeyValuePair<string, string>("id", contentType.Id.ToString())]);
      return Created(location, contentType);
    }

    return Ok(contentType);
  }
}
