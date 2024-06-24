using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Contents.Commands;
using Logitar.Cms.Core.Contents.Queries;
using Logitar.Cms.Web.Extensions;
using Logitar.Cms.Web.Models.Contents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/contents")]
public class ContentController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public ContentController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<ContentItem>> CreateAsync([FromBody] CreateContentPayload payload, CancellationToken cancellationToken)
  {
    ContentItem contentItem = await _pipeline.ExecuteAsync(new CreateContentCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("api/contents/{id}", new Dictionary<string, string>
    {
      ["id"] = contentItem.Id.ToString()
    });
    return Created(location, contentItem);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ContentItem>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ContentItem? contentItem = await _pipeline.ExecuteAsync(new ReadContentQuery(id), cancellationToken);
    return contentItem == null ? NotFound() : Ok(contentItem);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ContentItem>> SaveLocaleAsync(Guid id, [FromBody] SaveContentLocalePayload payload, Guid? languageId, CancellationToken cancellationToken)
  {
    ContentItem? contentItem = await _pipeline.ExecuteAsync(new SaveContentLocaleCommand(id, payload, languageId), cancellationToken);
    return contentItem == null ? NotFound() : Ok(contentItem);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<ContentLocale>>> SearchAsync([FromQuery] SearchContentsParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<ContentLocale> contentTypes = await _pipeline.ExecuteAsync(new SearchContentsQuery(parameters.ToPayload()), cancellationToken);
    return Ok(contentTypes);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<ContentItem>> UpdateLocaleAsync(Guid id, [FromBody] UpdateContentLocalePayload payload, Guid? languageId, CancellationToken cancellationToken)
  {
    ContentItem? contentItem = await _pipeline.ExecuteAsync(new UpdateContentLocaleCommand(id, payload, languageId), cancellationToken);
    return contentItem == null ? NotFound() : Ok(contentItem);
  }
}
