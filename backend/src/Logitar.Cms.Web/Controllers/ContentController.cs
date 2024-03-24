using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Contents.Commands;
using Logitar.Cms.Core.Contents.Queries;
using Logitar.Cms.Web.Extensions;
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
  public async Task<ActionResult<ContentItem>> CreateAsync([FromBody] CreateContentPayload payload, CancellationToken cancellationToken = default)
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
}
