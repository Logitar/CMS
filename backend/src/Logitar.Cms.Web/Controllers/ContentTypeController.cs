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
  public async Task<ActionResult<CmsContentType>> CreateAsync([FromBody] CreateContentTypePayload payload, CancellationToken cancellationToken)
  {
    CmsContentType contentType = await _pipeline.ExecuteAsync(new CreateContentTypeCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("api/contents/types/{id}", [new KeyValuePair<string, string>("id", contentType.Id.ToString())]);

    return Created(location, contentType);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<CmsContentType>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CmsContentType? contentType = await _pipeline.ExecuteAsync(new ReadContentTypeQuery(id, UniqueName: null), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }

  [HttpGet("unique-name:{uniqueName}")]
  public async Task<ActionResult<CmsContentType>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    CmsContentType? contentType = await _pipeline.ExecuteAsync(new ReadContentTypeQuery(Id: null, uniqueName), cancellationToken);
    return contentType == null ? NotFound() : Ok(contentType);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<CmsContentType>>> SearchAsync([FromQuery] SearchContentTypesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<CmsContentType> contentTypes = await _pipeline.ExecuteAsync(new SearchContentTypesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(contentTypes);
  }
}
