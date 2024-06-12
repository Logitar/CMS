using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Core;
using Logitar.Cms.Core.ContentTypes.Commands;
using Logitar.Cms.Core.ContentTypes.Queries;
using Logitar.Cms.Web.Extensions;
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
  public async Task<ActionResult<ContentsType>> CreateAsync([FromBody] CreateContentTypePayload payload, CancellationToken cancellationToken)
  {
    ContentsType contentType = await _pipeline.ExecuteAsync(new CreateContentTypeCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("api/contents/types/{id}", new Dictionary<string, string>
    {
      ["id"] = contentType.Id.ToString()
    });
    return Created(location, contentType);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ContentsType>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ContentsType? contentType = await _pipeline.ExecuteAsync(new ReadContentTypeQuery(id, UniqueName: null), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }

  [HttpGet("unique-name:{uniqueName}")]
  public async Task<ActionResult<ContentsType>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    ContentsType? contentType = await _pipeline.ExecuteAsync(new ReadContentTypeQuery(Id: null, uniqueName), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ContentsType>> ReplaceAsync(Guid id, [FromBody] ReplaceContentTypePayload payload, long? version, CancellationToken cancellationToken = default)
  {
    ContentsType? contentType = await _pipeline.ExecuteAsync(new ReplaceContentTypeCommand(id, payload, version), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<ContentsType>>> SearchAsync([FromQuery] SearchContentTypesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<ContentsType> contentTypes = await _pipeline.ExecuteAsync(new SearchContentTypesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(contentTypes);
  }
}
