using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Contents.Commands;
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
  public async Task<ActionResult<ContentModel>> CreateAsync([FromBody] CreateContentPayload payload, CancellationToken cancellationToken)
  {
    ContentModel content = await _pipeline.ExecuteAsync(new CreateContentCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("api/contents/{id}", [new KeyValuePair<string, string>("id", content.Id.ToString())]);
    return Created(location, content);
  }

  [HttpPut("{id}/invariant")]
  public async Task<ActionResult<ContentModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceContentLocalePayload payload, CancellationToken cancellationToken)
  {
    ContentModel? content = await _pipeline.ExecuteAsync(new CreateOrReplaceContentLocaleCommand(id, LanguageId: null, payload), cancellationToken);
    return GetActionResult(content);
  }

  [HttpPut("{contentId}/locales/{languageId}")]
  public async Task<ActionResult<ContentModel>> ReplaceAsync(Guid contentId, Guid languageId, [FromBody] CreateOrReplaceContentLocalePayload payload, CancellationToken cancellationToken)
  {
    ContentModel? content = await _pipeline.ExecuteAsync(new CreateOrReplaceContentLocaleCommand(contentId, languageId, payload), cancellationToken);
    return GetActionResult(content);
  }

  private ActionResult<ContentModel> GetActionResult(ContentModel? content)
  {
    return content == null ? NotFound() : Ok(content);
  }
}
